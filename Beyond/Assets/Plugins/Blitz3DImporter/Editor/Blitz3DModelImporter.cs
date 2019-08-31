using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using B3D;

public class Blitz3DModelImporter
{

    [MenuItem("Assets/Import B3D Model")]
    public static void ImportB3D()
    {
        var dir = "Assets";
        if (Selection.activeObject != null)
        {
            dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject) + "/");
            if (string.IsNullOrEmpty(dir))
            {
                dir = "Assets";
            }
        }

        if (!dir.EndsWith("/"))
        {
            dir += "/";
        }

        var file = EditorUtility.OpenFilePanel("Slect B3D File", "", "b3d");

        if (!File.Exists(file)) return;

        var fileName = Path.GetFileNameWithoutExtension(file);
        var importingAssetsDir = dir + fileName + "/";

        if (!Directory.Exists(importingAssetsDir))
        {
            Directory.CreateDirectory(importingAssetsDir);
        }

        B3D_BB3D b3d;

        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            b3d = br.ReadBB3D();
        }

        if (b3d == null)
        {
            return;
        }


        var root = new GameObject(fileName);

        var materials = new List<Material>();

        var fileDir = Path.GetDirectoryName(file);

        var texPaths = new Dictionary<B3D_TEXS, string>();
        foreach (var texture in b3d.textures)
        {

            try
            {
                string texPath = AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + texture.name);
                File.Copy(Path.Combine(fileDir, texture.name), texPath);
                texPaths.Add(texture, texPath);
            }
            catch
            { }
        }

        AssetDatabase.Refresh();

        foreach (var brush in b3d.brushes)
        {
            var material = new Material(Shader.Find("Diffuse"));
            material.name = brush.name;
            material.color = brush.color;
            try
            {
                string path;
                var texture = b3d.textures[brush.texture_id[0]];
                if (texPaths.TryGetValue(texture, out path))
                {
                    material.mainTexture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                    material.mainTextureOffset = texture.pos;
                    material.mainTextureScale = texture.scale;
                }
            }
            catch { }


            materials.Add(material);
            string materialAssetPath =
                 AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + material.name + ".asset");
            AssetDatabase.CreateAsset(material, materialAssetPath);
        }
        var goData = new Dictionary<B3D_NODE, GameObject>();
        root.transform.position = b3d.sceneNode.position;
        root.transform.rotation = b3d.sceneNode.rotation;
        root.transform.localScale = b3d.sceneNode.scale;
        CreateChilds(root, b3d.sceneNode, goData);



        foreach (var node1 in goData)
        {
            var go = node1.Value;
            var node = node1.Key;

            if (node.mesh == null) continue;

            bool isSkined = IsSkinned(node);
            var mesh = new Mesh();
            mesh.name = go.name;
            mesh.vertices = node.mesh.vertsData.vertices.ToArray();
            if (node.mesh.vertsData.containsNormals)
            {
                mesh.normals = node.mesh.vertsData.normals.ToArray();
            }
            if (node.mesh.vertsData.containsColors)
            {
                mesh.colors = node.mesh.vertsData.colors.ToArray();
            }
            if (node.mesh.vertsData.uv.Length > 0)
            {
                mesh.uv = node.mesh.vertsData.uv[0].ToArray();
            }

            var meshMaterials = new List<Material>();

            foreach (var surface in node.mesh.surfaces)
            {
                mesh.SetTriangles(surface.triangles.ToArray(), 0);

                int materialIndex = node.mesh.brush_id;
                if (surface.brush_id != -1)
                {
                    materialIndex = surface.brush_id;
                }
                if (materialIndex != -1)
                {
                    meshMaterials.Add(materials[materialIndex]);
                }
                else
                {
                    if (materials.Count>0)
                    {
                        meshMaterials.Add(materials[0]);
                    }
                }
            }

            if (isSkined)
            {
                var smr = go.AddComponent<SkinnedMeshRenderer>();
                smr.sharedMaterials = meshMaterials.ToArray();
                var boneNodes = new List<B3D_NODE>();
                FindBones(b3d.sceneNode, boneNodes);

                var bones = boneNodes.Select(t => goData[t].transform).ToList();


                smr.bones = bones.ToArray();

                mesh.bindposes = bones.Select(t => t.worldToLocalMatrix * go.transform.localToWorldMatrix).ToArray();

                var boneWeights = new List<BoneWeight>[mesh.vertexCount];

                for (int i = 0; i < boneWeights.Length; i++)
                {
                    boneWeights[i] = new List<BoneWeight>();
                }

                for (int i = 0; i < boneNodes.Count; i++)
                {
                    var boneNode = boneNodes[i];
                    foreach (var boneWeight in boneNode.bone.weights)
                    {
                        boneWeights[boneWeight.vertex_id].Add(new BoneWeight { boneIndex0 = i, weight0 = boneWeight.weight });
                    }
                }

                var wights = new BoneWeight[mesh.vertexCount];
                for (int i = 0; i < boneWeights.Length; i++)
                {
                    var orderdWeights = boneWeights[i].OrderByDescending(t => t.weight0).ToList();
                    var wight = new BoneWeight();
                    float len;
                    switch (orderdWeights.Count)
                    {
                        case 0:

                            break;
                        case 1:


                            wight.boneIndex0 = orderdWeights[0].boneIndex0;
                            wight.weight0 = orderdWeights[0].weight0;

                            break;
                        case 2:

                            wight.boneIndex0 = orderdWeights[0].boneIndex0;

                            wight.boneIndex1 = orderdWeights[1].boneIndex0;

                            len = Mathf.Sqrt(orderdWeights[0].weight0 * orderdWeights[0].weight0 +
                                             orderdWeights[1].weight0 * orderdWeights[1].weight0);
                            wight.weight0 = orderdWeights[0].weight0 / len;
                            wight.weight1 = orderdWeights[1].weight0 / len;
                            break;
                        case 3:

                            wight.boneIndex0 = orderdWeights[0].boneIndex0;
                            wight.boneIndex1 = orderdWeights[1].boneIndex0;
                            wight.boneIndex2 = orderdWeights[2].boneIndex0;

                            len = Mathf.Sqrt(orderdWeights[0].weight0 * orderdWeights[0].weight0 +
                                             orderdWeights[1].weight0 * orderdWeights[1].weight0 +
                                             orderdWeights[2].weight0 * orderdWeights[2].weight0);
                            wight.weight0 = orderdWeights[0].weight0 / len;
                            wight.weight1 = orderdWeights[1].weight0 / len;
                            wight.weight2 = orderdWeights[2].weight0 / len;

                            break;
                        default:

                            wight.boneIndex0 = orderdWeights[0].boneIndex0;
                            wight.boneIndex1 = orderdWeights[1].boneIndex0;
                            wight.boneIndex2 = orderdWeights[2].boneIndex0;
                            wight.boneIndex3 = orderdWeights[3].boneIndex0;

                            len = Mathf.Sqrt(orderdWeights[0].weight0 * orderdWeights[0].weight0 +
                                             orderdWeights[1].weight0 * orderdWeights[1].weight0 +
                                             orderdWeights[2].weight0 * orderdWeights[2].weight0 +
                                             orderdWeights[3].weight0 * orderdWeights[3].weight0);
                            wight.weight0 = orderdWeights[0].weight0 / len;
                            wight.weight1 = orderdWeights[1].weight0 / len;
                            wight.weight2 = orderdWeights[2].weight0 / len;
                            wight.weight3 = orderdWeights[3].weight0 / len;

                            break;
                    }
                    wights[i] = wight;
                }
                mesh.boneWeights = wights;
                smr.sharedMesh = mesh;
            }
            else
            {
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                go.AddComponent<MeshRenderer>().sharedMaterials = meshMaterials.ToArray();
            }

            string meshAssetPath =
                AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + node1.Key.name + ".asset");
            AssetDatabase.CreateAsset(mesh, meshAssetPath);
        }
        var animKeysPaths = new Dictionary<B3D_NODE, string>();

        FindAnimKeys(b3d.sceneNode, animKeysPaths, "");

        var curves = new List<AnimationCurveDescription>();

        foreach (var animPath in animKeysPaths)
        {
            var anim = FindAnim(animPath.Key);
            var fps = 1f / anim.anim.fps;
            var path = animPath.Value.Substring(animPath.Value.IndexOf("/") + 1);

            {
                // positions
                var positionKeysArrays = animPath.Key.animation_keys
                    .Where(t => t.containsPosition).Select(t => t.keyframes);
                var positionKeys = new List<B3D_KEYS_KEYFRAME>();
                foreach (var positionKeysArray in positionKeysArrays)
                {
                    positionKeys.AddRange(positionKeysArray);
                }
                positionKeys = positionKeys.OrderBy(t => t.frame).ToList();
                if (positionKeys.Count > 0)
                {
                    var positionX = new AnimationCurve();
                    var positionY = new AnimationCurve();
                    var positionZ = new AnimationCurve();

                    positionKeys.ForEach(t => positionX.AddKey(t.frame * fps, t.position.x));
                    positionKeys.ForEach(t => positionY.AddKey(t.frame * fps, t.position.y));
                    positionKeys.ForEach(t => positionZ.AddKey(t.frame * fps, t.position.z));

                    curves.Add(new AnimationCurveDescription { curve = positionX, path = path, propertyName = "m_LocalPosition.x" });
                    curves.Add(new AnimationCurveDescription { curve = positionY, path = path, propertyName = "m_LocalPosition.y" });
                    curves.Add(new AnimationCurveDescription { curve = positionZ, path = path, propertyName = "m_LocalPosition.z" });

                }
            }

            {
                // scales
                var scaleKeysArrays = animPath.Key.animation_keys
                    .Where(t => t.containsScale).Select(t => t.keyframes);
                var scaleKeys = new List<B3D_KEYS_KEYFRAME>();
                foreach (var scaleKeysArray in scaleKeysArrays)
                {
                    scaleKeys.AddRange(scaleKeysArray);
                }
                scaleKeys = scaleKeys.OrderBy(t => t.frame).ToList();
                if (scaleKeys.Count > 0)
                {
                    var scaleX = new AnimationCurve();
                    var scaleY = new AnimationCurve();
                    var scaleZ = new AnimationCurve();

                    scaleKeys.ForEach(t => scaleX.AddKey(t.frame * fps, t.scale.x));
                    scaleKeys.ForEach(t => scaleY.AddKey(t.frame * fps, t.scale.y));
                    scaleKeys.ForEach(t => scaleZ.AddKey(t.frame * fps, t.scale.z));

                    curves.Add(new AnimationCurveDescription { curve = scaleX, path = path, propertyName = "m_LocalScale.x" });
                    curves.Add(new AnimationCurveDescription { curve = scaleY, path = path, propertyName = "m_LocalScale.y" });
                    curves.Add(new AnimationCurveDescription { curve = scaleZ, path = path, propertyName = "m_LocalScale.z" });

                }
            }

            {
                // rotations
                var rotKeysArrays = animPath.Key.animation_keys
                    .Where(t => t.containsRotation).Select(t => t.keyframes);
                var rotKeys = new List<B3D_KEYS_KEYFRAME>();
                foreach (var rotKeysArray in rotKeysArrays)
                {
                    rotKeys.AddRange(rotKeysArray);
                }
                rotKeys = rotKeys.OrderBy(t => t.frame).ToList();
                if (rotKeys.Count > 0)
                {
                    var rotX = new AnimationCurve();
                    var rotY = new AnimationCurve();
                    var rotZ = new AnimationCurve();
                    var rotW = new AnimationCurve();

                    rotKeys.ForEach(t => rotX.AddKey(t.frame * fps, t.rotation.x));
                    rotKeys.ForEach(t => rotY.AddKey(t.frame * fps, t.rotation.y));
                    rotKeys.ForEach(t => rotZ.AddKey(t.frame * fps, t.rotation.z));
                    rotKeys.ForEach(t => rotW.AddKey(t.frame * fps, t.rotation.w));

                    curves.Add(new AnimationCurveDescription { curve = rotX, path = path, propertyName = "m_LocalRotation.x" });
                    curves.Add(new AnimationCurveDescription { curve = rotY, path = path, propertyName = "m_LocalRotation.y" });
                    curves.Add(new AnimationCurveDescription { curve = rotZ, path = path, propertyName = "m_LocalRotation.z" });
                    curves.Add(new AnimationCurveDescription { curve = rotW, path = path, propertyName = "m_LocalRotation.w" });
                }
            }
        }

        var clip = new AnimationClip();
        foreach (var animationCurveDescription in curves)
        {
            clip.SetCurve(animationCurveDescription.path,
                typeof(Transform),
                animationCurveDescription.propertyName,
                animationCurveDescription.curve);
        }

        clip.EnsureQuaternionContinuity();

        var animation = root.AddComponent<Animation>();
        clip.name = "Track001";

        string clipAssetPath =
                AssetDatabase.GenerateUniqueAssetPath(importingAssetsDir + fileName + "_" + clip.name + ".asset");
        AssetDatabase.CreateAsset(clip, clipAssetPath);
        animation.clip = clip;
        animation.playAutomatically = true;
        string prefabPath =
                AssetDatabase.GenerateUniqueAssetPath(dir + fileName + ".prefab");
        var prefab = EditorUtility.CreateEmptyPrefab(prefabPath);
        EditorUtility.ReplacePrefab(root, prefab, ReplacePrefabOptions.ConnectToPrefab);
        AssetDatabase.Refresh();
    }

    public class AnimationCurveDescription
    {
        public string path;
        public string propertyName;
        public AnimationCurve curve;
    }

    private static B3D_NODE FindAnim(B3D_NODE keysNode)
    {
        var current = keysNode;

        while (true)
        {
            if (current == null) break;

            if (current.anim != null) return current;

            current = current.parent;
        }

        return null;
    }


    private static void FindAnimKeys(B3D_NODE node, Dictionary<B3D_NODE, string> anims, string path)
    {
        string currentPath;
        if (path == "")
        {
            currentPath = node.name;
        }
        else
        {
            currentPath = path + "/" + node.name;
        }

        if (node.animation_keys.Count > 0)
        {
            anims.Add(node, currentPath);
        }

        foreach (var child in node.childs)
        {
            FindAnimKeys(child, anims, currentPath);
        }
    }

    private static void FindBones(B3D_NODE node, List<B3D_NODE> bones)
    {
        if (node.bone != null)
        {
            bones.Add(node);


        }

        foreach (var child in node.childs)
        {
            FindBones(child, bones);
        }
    }

    private static bool IsSkinned(B3D_NODE node)
    {
        if (node.bone != null) return true;

        foreach (var child in node.childs)
        {
            if (child.bone != null)
            {
                return true;
            }
            bool isChildSkinned = IsSkinned(child);
            if (isChildSkinned) return true;
        }

        return false;
    }

    static void CreateChilds(GameObject go, B3D_NODE node, Dictionary<B3D_NODE, GameObject> goData)
    {
        goData[node] = go;


        foreach (var child in node.childs)
        {

            if (child is B3DEXT_OMNILIGHT)
            {
                var light = go.AddComponent<Light>();
                light.color = (child as B3DEXT_OMNILIGHT).color;
                light.type = LightType.Point;
            }
            else if (child is B3DEXT_DIRLIGHT)
            {
                var light = go.AddComponent<Light>();
                light.color = (child as B3DEXT_DIRLIGHT).color;
                light.type = LightType.Directional;
            }
            else if (child is B3DEXT_SPOTLIGHT)
            {
                var light = go.AddComponent<Light>();
                light.color = (child as B3DEXT_SPOTLIGHT).color;
                light.type = LightType.Spot;
            }
            else if (child is B3DEXT_CAMERA)
            {
                var camera = go.AddComponent<Camera>();
                camera.fieldOfView = (child as B3DEXT_CAMERA).fov;
            }
            else
            {
                var childGO = new GameObject(child.name);
                childGO.transform.parent = go.transform;
                childGO.transform.localPosition = child.position;
                childGO.transform.localScale = child.scale;
                childGO.transform.localRotation = child.rotation;
                CreateChilds(childGO, child, goData);
            }


        }
    }

}

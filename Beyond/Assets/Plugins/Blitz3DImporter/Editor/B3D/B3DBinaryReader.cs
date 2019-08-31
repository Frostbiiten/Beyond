using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace B3D
{
    public static class B3DBinaryReader
    {
        public static string ReadCString(this BinaryReader br)
        {
            string str = "";
            while (true)
            {
                byte b = br.ReadByte();
                if (b == 0)
                {
                    break;
                }
                str += (char)b;
            }

            return str;
        }

        public static Color ReadColor(this BinaryReader br)
        {
            var color = new Color();
            color.r = br.ReadSingle();
            color.g = br.ReadSingle();
            color.b = br.ReadSingle();
            color.a = br.ReadSingle();
            return color;
        }

        public static Vector2 ReadVector2(this BinaryReader br)
        {
            var vector2 = new Vector2();
            vector2.x = br.ReadSingle();
            vector2.y = br.ReadSingle();
            return vector2;
        }

        public static Vector3 ReadVector3(this BinaryReader br)
        {
            var v3 = new Vector3();
            v3.x = br.ReadSingle();
            v3.y = br.ReadSingle();
            v3.z = br.ReadSingle();
            return v3;
        }

        public static Quaternion ReadQuaternion(this BinaryReader br)
        {
            var q = new Quaternion();
            q.w = br.ReadSingle();
            q.x = br.ReadSingle();
            q.y = br.ReadSingle();
            q.z = br.ReadSingle();
            q = Quaternion.Inverse(q);
            return q;
        }


        public static B3D_CHNK ReadChunk(this BinaryReader br)
        {
            B3D_CHNK chunk = new B3D_CHNK();
            string str = "";

            chunk.name = Encoding.ASCII.GetString(br.ReadBytes(4));
            int chunkLength = br.ReadInt32();
            chunk.bytes = br.ReadBytes(chunkLength);
            return chunk;
        }

        public static List<B3D_CHNK> ReadChunks(this BinaryReader br)
        {
            var chunks = new List<B3D_CHNK>();
            while (true)
            {
                try
                {
                    var chunk = ReadChunk(br);
                    chunks.Add(chunk);
                }
                catch
                {
                    break;
                }
            }
            return chunks;
        }

        public static List<B3D_TEXS> ReadTEXS(this B3D_CHNK chunk)
        {
            var textures = new List<B3D_TEXS>();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                while (true)
                {
                    try
                    {
                        var tex = new B3D_TEXS();
                        tex.name = br.ReadCString();
                        tex.flags = br.ReadInt32();
                        tex.blend = br.ReadInt32();
                        tex.pos= br.ReadVector2();
                        tex.scale = br.ReadVector2();
                        tex.rotation = br.ReadSingle();
                        textures.Add(tex);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return textures;
        }

        public static List<B3D_BRUS> ReadBRUS(this B3D_CHNK chunk)
        {
            var brushes = new List<B3D_BRUS>();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                int n_texes = br.ReadInt32();

                while (true)
                {
                    try
                    {
                        var brush = new B3D_BRUS();
                        brush.name = br.ReadCString();
                        brush.color = br.ReadColor();
                        brush.shininess = br.ReadSingle();
                        brush.blend = br.ReadInt32();
                        brush.fx = br.ReadInt32();
                        brush.texture_id = new int[n_texes];
                        for (int i = 0; i < brush.texture_id.Length; i++)
                        {
                            brush.texture_id[i] = br.ReadInt32();
                        }
                        brushes.Add(brush);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return brushes;
        }

        public static B3D_NODE ReadNODE(this B3D_CHNK chunk)
        {
            var node = new B3D_NODE();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                var name = br.ReadCString();

                if (name == "B3DEXT_CAMERA")
                {
                    node = new B3DEXT_CAMERA();
                }
                else if (name == "B3DEXT_OMNILIGHT")
                {
                    node = new B3DEXT_OMNILIGHT();
                }
                else if (name == "B3DEXT_DIRLIGHT")
                {
                    node = new B3DEXT_DIRLIGHT();
                }
                else if (name == "B3DEXT_SPOTLIGHT")
                {
                    node = new B3DEXT_SPOTLIGHT();
                }
                else if (name == "B3DEXT_RANGE")
                {
                    node = new B3DEXT_RANGE();
                }
                else if (name == "B3DEXT_FOV")
                {
                    node = new B3DEXT_FOV();
                }
                else if (name == "B3DEXT_VISIBILITY")
                {
                    node = new B3DEXT_VISIBILITY();
                }

                node.name = name;
                node.position = br.ReadVector3();
                node.scale = br.ReadVector3();
                node.rotation = br.ReadQuaternion();

                foreach (var childChunk in br.ReadChunks())
                {
                    switch (childChunk.name)
                    {
                        case "NODE":
                            node.childs.Add(childChunk.ReadNODE());
                            break;
                        case "BONE":
                            node.bone = childChunk.ReadBONE();
                            break;
                        case "MESH":
                            node.mesh = childChunk.ReadMESH();
                            break;
                        case "ANIM":
                            node.anim = childChunk.ReadANIM();
                            break;
                        case "KEYS":
                            node.animation_keys.Add(childChunk.ReadKEYS());
                            break;
                    }
                }

                foreach (var child in node.childs)
                {
                    child.parent = node;
                }
            }
            return node;
        }

        public static B3D_ANIM ReadANIM(this B3D_CHNK chunk)
        {
            var anim = new B3D_ANIM();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                anim.flags = br.ReadInt32();
                anim.frames = br.ReadInt32();
                anim.fps = br.ReadSingle();
            }
            return anim;
        }

        public static B3D_KEYS ReadKEYS(this B3D_CHNK chunk)
        {
            var keys = new B3D_KEYS();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                keys.flags = br.ReadInt32();

                while (true)
                {
                    try
                    {
                        var keyframe = new B3D_KEYS_KEYFRAME();
                        keyframe.frame = br.ReadInt32();
                        if (keys.containsPosition)
                        {
                            keyframe.position = br.ReadVector3();
                        }

                        if (keys.containsScale)
                        {
                            keyframe.scale = br.ReadVector3();
                        }

                        if (keys.containsRotation)
                        {
                            keyframe.rotation = br.ReadQuaternion();
                        }
                        keys.keyframes.Add(keyframe);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return keys;
        }

        public static B3D_BONE ReadBONE(this B3D_CHNK chunk)
        {
            var bone = new B3D_BONE();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                while (true)
                {
                    try
                    {
                        var boneWeight = new B3D_BONE_WEIGHT();
                        boneWeight.vertex_id = br.ReadInt32();
                        boneWeight.weight = br.ReadSingle();
                        bone.weights.Add(boneWeight);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return bone;
        }

        public static B3D_VRTS ReadVRTS(this B3D_CHNK chunk)
        {
            var vrts = new B3D_VRTS();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                vrts.flags = br.ReadInt32();
                vrts.tex_coord_sets = br.ReadInt32();
                vrts.tex_coord_set_size = br.ReadInt32();
                vrts.uv = new List<Vector2>[vrts.tex_coord_sets];
                for (int i = 0; i < vrts.uv.Length; i++)
                {
                    vrts.uv[i] = new List<Vector2>();
                }

                while (true)
                {
                    try
                    {
                        Vector3 vertex = br.ReadVector3();
                        vrts.vertices.Add(vertex);

                        if (vrts.containsNormals)
                        {
                            Vector3 normal = br.ReadVector3();
                            vrts.normals.Add(normal);
                        }
                        if (vrts.containsColors)
                        {
                            var color = br.ReadColor();
                            vrts.colors.Add(color);
                        }

                        for (int i = 0; i < vrts.tex_coord_sets; i++)
                        {
                            var uvData = new float[vrts.tex_coord_set_size];
                            for (int j = 0; j < vrts.tex_coord_set_size; j++)
                            {
                                uvData[j] = br.ReadSingle();
                            }
                            vrts.uv[i].Add(new Vector2(uvData[0], uvData[1]));
                        }


                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return vrts;

        }

        public static B3D_TRIS ReadTRIS(this B3D_CHNK chunk)
        {
            var triangles = new B3D_TRIS();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                triangles.brush_id = br.ReadInt32();
                var triangle = new int[3];
                while (true)
                {
                    try
                    {
                        triangle[0] = br.ReadInt32();
                        triangle[1] = br.ReadInt32();
                        triangle[2] = br.ReadInt32();
                        triangles.triangles.AddRange(triangle);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            return triangles;
        }

        public static B3D_MESH ReadMESH(this B3D_CHNK chunk)
        {
            var mesh = new B3D_MESH();
            using (var br = new BinaryReader(new MemoryStream(chunk.bytes)))
            {
                mesh.brush_id = br.ReadInt32();

                foreach (var childChunk in br.ReadChunks())
                {
                    switch (childChunk.name)
                    {
                        case "VRTS":
                            mesh.vertsData = childChunk.ReadVRTS();

                            break;
                        case "TRIS":
                            mesh.surfaces.Add(childChunk.ReadTRIS());
                            break;
                    }
                }
            }
            return mesh;
        }

        public static B3D_BB3D ReadBB3D(this BinaryReader binaryReader)
        {
            var bb3d = new B3D_BB3D();
            B3D_CHNK bb3dChunk;
            try
            {
                bb3dChunk = binaryReader.ReadChunk();
            }
            catch
            {
                Debug.Log("Error while reading BB3D chunk.");
                return null;
            }


            if (bb3dChunk != null && bb3dChunk.name == "BB3D")
            {
                using (var br = new BinaryReader(new MemoryStream(bb3dChunk.bytes)))
                {
                    bb3d.version = br.ReadInt32();

                    if (bb3d.version == 1)
                    {

                        foreach (var chunk in br.ReadChunks())
                        {
                            switch (chunk.name)
                            {
                                case "TEXS":
                                    bb3d.textures = chunk.ReadTEXS();
                                    break;
                                case "BRUS":
                                    bb3d.brushes = chunk.ReadBRUS();
                                    break;
                                case "NODE":
                                    bb3d.sceneNode = chunk.ReadNODE();
                                    break;
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("Unknown b3d version!");
                        return null;
                    }
                }

                return bb3d;
            }

            return null;
        }

    }
}
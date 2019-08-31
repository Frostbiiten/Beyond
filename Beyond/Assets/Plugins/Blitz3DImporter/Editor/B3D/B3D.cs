using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace B3D
{
    public class B3D_BB3D
    {
        public int version;
        public List<B3D_TEXS> textures = new List<B3D_TEXS>();
        public List<B3D_BRUS> brushes = new List<B3D_BRUS>();
        public B3D_NODE sceneNode;
    }

    public class B3D_ANIM
    {
        public int flags;  //unused: default=0
        public int frames; //how many frames in anim
        public float fps;  // default=60
    }

    public class B3D_BONE
    {
        public List<B3D_BONE_WEIGHT> weights = new List<B3D_BONE_WEIGHT>();
    }

    public class B3D_BONE_WEIGHT
    {
        public int vertex_id; //vertex affected by this bone
        public float weight; //how much the vertex is affected
    }

    public class B3D_BRUS
    {
        public string name;
        public Color color = new Color(1, 1, 1, 1);
        public float shininess;
        public int blend, fx;
        public int[] texture_id;
    }

    public class B3D_CHNK
    {
        public string name;
        public byte[] bytes;
    }

    public class B3D_KEYS
    {
        public int flags; //1=position, 2=scale, 4=rotation
        public List<B3D_KEYS_KEYFRAME> keyframes = new List<B3D_KEYS_KEYFRAME>();

        public bool containsPosition
        {
            get
            {
                return (flags & 1) != 0;
            }
        }

        public bool containsScale
        {
            get
            {
                return (flags & 2) != 0;
            }
        }

        public bool containsRotation
        {
            get
            {
                return (flags & 4) != 0;
            }
        }
    }

    public class B3D_KEYS_KEYFRAME
    {
        public int frame;
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;

    }

    public class B3D_MESH
    {
        public int brush_id = -1;
        public B3D_VRTS vertsData;
        public List<B3D_TRIS> surfaces = new List<B3D_TRIS>();
    }

    public class B3D_NODE
    {
        public string name;
        public Vector3 position = Vector3.zero;
        public Vector3 scale = Vector3.one;
        public Quaternion rotation = Quaternion.identity;

        public B3D_BONE bone;
        public B3D_MESH mesh;
        public List<B3D_NODE> childs = new List<B3D_NODE>();
        public B3D_ANIM anim;
        public List<B3D_KEYS> animation_keys = new List<B3D_KEYS>();
        public B3DEXT_CAMERA camera;
        public B3DEXT_LIGHT light;
        public B3D_NODE parent;
    }

    public class B3D_TEXS
    {
        public string name;
        public int flags;
        public int blend;
        public Vector2 pos;
        public Vector2 scale;
        public float rotation;
    }

    public class B3D_TRIS
    {
        public int brush_id = -1; //brush applied to these TRIs: default=-1

        public List<int> triangles = new List<int>();
    }

    public class B3D_VRTS
    {
        public int flags; //1=normal values present, 2=rgba values present
        public int tex_coord_sets = 1; //texture coords per vertex (eg: 1 for simple U/V) max=8
        public int tex_coord_set_size; //components per set (eg: 2 for simple U/V) max=4

        public bool containsNormals
        {
            get
            {
                return (flags & 1) != 0;
            }
        }

        public bool containsColors
        {
            get
            {
                return (flags & 2) != 0;
            }
        }

        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Color> colors = new List<Color>();
        public List<Vector2>[] uv;
    }
}

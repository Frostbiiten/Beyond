using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace B3D
{
    public class B3DEXT_CAMERA : B3D_NODE
    {
        public float fov
        {
            get { return position.x; }
        }
    }

    public class B3DEXT_LIGHT:B3D_NODE
    {
        public Color color
        {
            get
            {
                return new Color(position.x, position.y, position.z);
            }
        }
    
    }

    public class B3DEXT_OMNILIGHT : B3DEXT_LIGHT
    {

    }

    public class B3DEXT_DIRLIGHT : B3DEXT_LIGHT
    {

    }

    public class B3DEXT_SPOTLIGHT : B3DEXT_LIGHT
    {

    }

    public class B3DEXT_RANGE : B3D_NODE
    {
        public float LightRange
        {
            get
            {
                return position.x;
            }
        }
    }

    public class B3DEXT_FOV : B3D_NODE
    {
        public float  InnerAngle
        {
            get
            {
                return position.x;
            }
        }

        public float OutterAngle
        {
            get
            {
                return position.y;
            }
        }
    }

    public class B3DEXT_VISIBILITY : B3D_NODE
    {
        public float alpha
        {
            get { return position.x; }
        }
    }
}
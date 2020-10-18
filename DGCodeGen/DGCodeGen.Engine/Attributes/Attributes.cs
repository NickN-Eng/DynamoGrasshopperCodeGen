using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DGCodeGen.Attributes
{
    /// <summary>
    /// A function to be ported to both Dynamo AND Grasshopper
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class GhGuid : Attribute
    {
        public string Guid;

        public GhGuid(string guid)
        {
            Guid = guid;
        }
    }

    /// <summary>
    /// Tells DGCodeGen to port this method to both Dynamo AND Grasshopper
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DGFunc : Attribute
    {
        public string Name;
        public string NickName;
        public string Parent;
        public string Grandparent;

        public DGFunc(string name, string nickName, string parent, string grandparent)
        {
            Name = name;
            NickName = nickName;
            Parent = parent;
            Grandparent = grandparent;
        }

        public DyFunc ToDyFunc() => new DyFunc(Name, Parent, Grandparent);
        public GhFunc ToGhFunc() => new GhFunc(Name, NickName, Parent, Grandparent);
    }

    /// <summary>
    /// Tells DGCodeGen to port this method to Dynamo
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DyFunc : Attribute
    {
        public string Name;
        public string Parent;
        public string Grandparent;

        public DyFunc(string name, string parent, string grandparent)
        {
            Name = name;
            Parent = parent;
            Grandparent = grandparent;
        }
    }

    /// <summary>
    /// Tells DGCodeGen to port this method to Grasshopper
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GhFunc : Attribute
    {
        public string Name;
        public string NickName;
        public string Subcategory;
        public string Category;

        public GhFunc(string name, string nickName, string category, string subcategory)
        {
            Name = name;
            NickName = nickName;
            Subcategory = subcategory;
            Category = category;
        }
    }

    /// <summary>
    /// Define a description which will be used by both Dynamo (xml comment) and Grasshopper (node description).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DGDescription : Attribute
    {
        public string Description;

        public DGDescription(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// Define a node input which will be used by both Dynamo and Grasshopper
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DGInput : Attribute
    {
        public string Name;
        public string NickNameGh;
        public string Description;

        public DGInput(string name, string nickNameGh, string description)
        {
            Name = name;
            NickNameGh = nickNameGh;
            Description = description;
        }
    }


    /// <summary>
    /// Define a node output which will be used by both Dynamo and Grasshopper
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DGOutput : Attribute
    {
        public string Name;
        public string NickNameGh;
        public string Description;

        public DGOutput(string name, string nickNameGh, string description)
        {
            Name = name;
            NickNameGh = nickNameGh;
            Description = description;
        }
    }

    /// <summary>
    /// Define a dataclass which will be copied into the Dynamo library
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DyDataClass : Attribute
    {
        public DyDataClass()
        {
        }
    }

    /// <summary>
    /// Define a dataclass which will be copied into the Grasshopper library
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GhDataClass : Attribute
    {
        public string NickName;
        public string Subcategory;
        public string Category;

        public GhDataClass(string nickName, string subcategory, string category)
        {
            NickName = nickName;
            Subcategory = subcategory;
            Category = category;
        }
    }
}

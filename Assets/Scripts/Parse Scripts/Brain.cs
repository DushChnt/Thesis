using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;

[ParseClassName("Brain")]
public class Brain : ParseObject {

    [ParseFieldName("name")]
    public string Name
    {
        get { return GetProperty<string>("Name"); }
        set { SetProperty<string>(value, "Name"); }
    }

    [ParseFieldName("parentId")]
    public string ParentId
    {
        get { return GetProperty<string>("ParentId"); }
        set { SetProperty<string>(value, "ParentId"); }
    }

    [ParseFieldName("description")]
    public string Description
    {
        get { return GetProperty<string>("Description"); }
        set { SetProperty<string>(value, "Description"); }
    }

    [ParseFieldName("userId")]
    public string UserId
    {
        get { return GetProperty<string>("UserId"); }
        set { SetProperty<string>(value, "UserId"); }
    }
    
    public List<Brain> Children = new List<Brain>();
    
    public bool IsNewBrain;

    public Brain Branch()
    {
        Brain brain = new Brain();

        brain.Name = "(Branch) " + this.Name;
        brain.Description = this.Description;
        brain.ParentId = this.ObjectId;
        brain.UserId = this.UserId;
        brain.IsNewBrain = true;

        return brain;
    }

    //public override bool Equals(object obj)
    //{
    //    if (obj == null)
    //    {
    //        return false;
    //    }
    //    Brain other = obj as Brain;
    //    if (Id == null)
    //    {
    //        return false;
    //    }
    //    return this.Id.Equals(other.Id);
    //}
}

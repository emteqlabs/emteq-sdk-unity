using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmteqLabs
{
    [RequireComponent(typeof(Renderer), typeof(Collider))]
    public class TrackedObject : MonoBehaviour
    {
        private static HashSet<string> _uniqueObjectIdSet = new HashSet<string>();

        public string ObjectId;

        private void Awake()
        {
            ObjectId = GetUniqueObjectId();
        }

        private void OnDestroy()
        {
            _uniqueObjectIdSet.Remove(ObjectId);
        }

        private string GetUniqueObjectId()
        {
            string id = ObjectId;
            
            //if ID is not set, use the gameobject name
            if (id.Equals(""))
            {
                id = name;
            }

            int count = 0;
            while (_uniqueObjectIdSet.Contains(id))
            {
                //if another object has the same id, rename it using Unity's naming convention for object copies
                id = $"{id} ({++count})";
            }
            _uniqueObjectIdSet.Add(id);
            
            // rename gameObject if another has the same name.
            // if you don't want the gameObject's name modified, use the _objectId instead
            if (count > 0)
            {
                name = id;
            }

            return id;
        }

        public void EnterGaze(string objectName)
        {
            EmteqManager.StartDataSection($"Gaze:{objectName}");
        }
        
        public void ExitGaze(string objectName)
        {
            EmteqManager.EndDataSection($"Gaze:{objectName}");
        }
    }
}

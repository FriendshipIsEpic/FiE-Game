/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for a CGF data structer 2D.
*******************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircularGravityForce
{
    public class CGFCollection2D
    {
        public class EffectedObject
        {
            private Rigidbody2D rigidbody;
            public Rigidbody2D _rigidbody
            {
                get { return rigidbody; }
                set { rigidbody = value; }
            }

            private object objectHistory;
            public object ObjectHistory
            {
                get { return objectHistory; }
                set { objectHistory = value; }
            }

            private float gameTime;
            public float GameTime
            {
                get { return gameTime; }
                set { gameTime = value; }
            }

            private float timeEffected;
            public float TimeEffected
            {
                get { return timeEffected; }
                set { timeEffected = value; }
            }
        }

        private List<EffectedObject> effectedObjects;
        public List<EffectedObject> EffectedObjects
        {
            get { return effectedObjects; }
            set { effectedObjects = value; }
        }

        public CGFCollection2D()
        {
            EffectedObjects = new List<EffectedObject>();
        }

        public void Add(Rigidbody2D rigid, float gameTime, float timeEffected)
        {
            bool foundFlag = false;
            for (int i = 0; i < EffectedObjects.Count; i++)
            {
                if (EffectedObjects[i]._rigidbody == rigid)
                {
                    EffectedObjects[i].GameTime = gameTime;
                    EffectedObjects[i].TimeEffected = timeEffected;

                    foundFlag = true;

                    break;
                }
            }

            if (!foundFlag)
            {
                var item = new EffectedObject();
                item._rigidbody = rigid;

                item.GameTime = gameTime;
                item.TimeEffected = timeEffected;
                SetEffectedObjectHistory(rigid, item);
                EffectedObjects.Add(item);
            }

            UpdateEffectedObject(rigid);
        }

        public void Sync()
        {
            if (EffectedObjects.Count > 0)
            {
                for (int i = 0; i < EffectedObjects.Count; i++)
                {
                    if ((Time.time - EffectedObjects[i].GameTime) > EffectedObjects[i].TimeEffected)
                    {
                        ResetEffectedObject(EffectedObjects[i]);
                    }
                }
            }
        }

        public virtual void SetEffectedObjectHistory(Rigidbody2D rigid, EffectedObject effectedObject)
        {

        }

        public virtual void UpdateEffectedObject(Rigidbody2D rigid)
        {

        }

        public virtual void ResetEffectedObject(EffectedObject effectedObject)
        {

        }
    }
}
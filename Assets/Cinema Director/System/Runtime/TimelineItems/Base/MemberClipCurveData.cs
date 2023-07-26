using CinemaDirector.Helpers;
using System;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    [System.Serializable]
    public class MemberClipCurveData
    {
        public string Type;
        public string PropertyName;
        public bool IsProperty = true;
        public PropertyTypeInfo PropertyType = PropertyTypeInfo.None;

        public AnimationCurve Curve1 = new AnimationCurve();
        public AnimationCurve Curve2 = new AnimationCurve();
        public AnimationCurve Curve3 = new AnimationCurve();
        public AnimationCurve Curve4 = new AnimationCurve();

        //private object cachedProperty;

        public AnimationCurve GetCurve(int i)
        {
            if (i == 1) return Curve2;
            else if (i == 2) return Curve3;
            else if (i == 3) return Curve4;
            else return Curve1;
        }

        public void Initialize(GameObject Actor)
        {
            //Component component = Actor.GetComponent(Type);
            //cachedProperty = getCurrentValue(component);
        }

        internal void Reset(GameObject Actor)
        {
            //Component component = Actor.GetComponent(Type);
            //if (component == null || PropertyName == string.Empty) return;

            //Type type = component.GetType();
            //if (IsProperty)
            //{
            //    PropertyInfo propertyInfo = type.GetProperty(PropertyName);
            //    propertyInfo.SetValue(component, cachedProperty, null);
            //}
            //else
            //{
            //    FieldInfo fieldInfo = type.GetField(PropertyName);
            //    fieldInfo.SetValue(component, cachedProperty);
            //}
        }

        internal object getCurrentValue(Component component)
        {
            if (component == null || this.PropertyName == string.Empty) return null;
            Type type = component.GetType();
            object value = null;
            if (this.IsProperty)
            {
                PropertyInfo propertyInfo = type.GetProperty(this.PropertyName);
                value = propertyInfo.GetValue(component, null);
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(this.PropertyName);
                value = fieldInfo.GetValue(component);
            }
            return value;
        }

    }
}
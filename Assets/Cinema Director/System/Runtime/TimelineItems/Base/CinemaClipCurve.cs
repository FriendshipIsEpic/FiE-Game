using System;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// Curve Clip Items tie Actor component data to animation curves, so that they can be controlled
    /// by curves over time.
    /// </summary>
    public abstract class CinemaClipCurve : TimelineAction
    {
        // The curve data
        [SerializeField]
        private List<MemberClipCurveData> curveData = new List<MemberClipCurveData>();

        /// <summary>
        /// Return the Curve Clip data.
        /// </summary>
        public List<MemberClipCurveData> CurveData
        {
            get { return curveData; }
        }

        protected virtual void initializeClipCurves(MemberClipCurveData data, Component component) { }

        public void AddClipCurveData(Component component, string name, bool isProperty, Type type)
        {
            MemberClipCurveData data = new MemberClipCurveData();
            data.Type = component.GetType().Name;
            data.PropertyName = name;
            data.IsProperty = isProperty;
            data.PropertyType = UnityPropertyTypeInfo.GetMappedType(type);
            initializeClipCurves(data, component);
            curveData.Add(data);
        }

        protected object evaluate(MemberClipCurveData memberData, float time)
        {
            object value = null;
            switch (memberData.PropertyType)
            {
                case PropertyTypeInfo.Color:
                    Color c;
                    c.r = memberData.Curve1.Evaluate(time);
                    c.g = memberData.Curve2.Evaluate(time);
                    c.b = memberData.Curve3.Evaluate(time);
                    c.a = memberData.Curve4.Evaluate(time);
                    value = c;
                    break;

                case PropertyTypeInfo.Double:
                case PropertyTypeInfo.Float:
                case PropertyTypeInfo.Int:
                case PropertyTypeInfo.Long:
                    value = memberData.Curve1.Evaluate(time);
                    break;

                case PropertyTypeInfo.Quaternion:
                    Quaternion q;
                    q.x = memberData.Curve1.Evaluate(time);
                    q.y = memberData.Curve2.Evaluate(time);
                    q.z = memberData.Curve3.Evaluate(time);
                    q.w = memberData.Curve4.Evaluate(time);
                    value = q;
                    break;

                case PropertyTypeInfo.Vector2:
                    Vector2 v2;
                    v2.x = memberData.Curve1.Evaluate(time);
                    v2.y = memberData.Curve2.Evaluate(time);
                    value = v2;
                    break;

                case PropertyTypeInfo.Vector3:
                    Vector3 v3;
                    v3.x = memberData.Curve1.Evaluate(time);
                    v3.y = memberData.Curve2.Evaluate(time);
                    v3.z = memberData.Curve3.Evaluate(time);
                    value = v3;
                    break;

                case PropertyTypeInfo.Vector4:
                    Vector4 v4;
                    v4.x = memberData.Curve1.Evaluate(time);
                    v4.y = memberData.Curve2.Evaluate(time);
                    v4.z = memberData.Curve3.Evaluate(time);
                    v4.w = memberData.Curve4.Evaluate(time);
                    value = v4;
                    break;
            }
            return value;
        }

        private void updateKeyframeTime(float oldTime, float newTime)
        {

            foreach (MemberClipCurveData data in curveData)
            {
                int curveCount = UnityPropertyTypeInfo.GetCurveCount(data.PropertyType);
                for (int i = 0; i < curveCount; i++)
                {
                    AnimationCurve animationCurve = data.GetCurve(i);
                    for (int j = 0; j < animationCurve.length; j++)
                    {
                        Keyframe kf = animationCurve.keys[j];

                        if (Mathf.Abs(kf.time - oldTime) < 0.00001)
                        {
                            Keyframe newKeyframe = new Keyframe(newTime, kf.value, kf.inTangent, kf.outTangent);
                            animationCurve.MoveKey(j, newKeyframe);
                        }
                    }
                }
            }
        }

        public void TranslateCurves(float amount)
        {
            base.Firetime += amount;
            foreach (MemberClipCurveData data in curveData)
            {
                int curveCount = UnityPropertyTypeInfo.GetCurveCount(data.PropertyType);
                for (int i = 0; i < curveCount; i++)
                {
                    AnimationCurve animationCurve = data.GetCurve(i);
                    if (amount > 0)
                    {
                        for (int j = animationCurve.length - 1; j >= 0; j--)
                        {
                            Keyframe kf = animationCurve.keys[j];
                            Keyframe newKeyframe = new Keyframe(kf.time + amount, kf.value, kf.inTangent, kf.outTangent);
                            animationCurve.MoveKey(j, newKeyframe);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < animationCurve.length; j++)
                        {
                            Keyframe kf = animationCurve.keys[j];
                            Keyframe newKeyframe = new Keyframe(kf.time + amount, kf.value, kf.inTangent, kf.outTangent);
                            animationCurve.MoveKey(j, newKeyframe);
                        }
                    }
                }
            }
        }

        public void AlterFiretime(float firetime, float duration)
        {
            updateKeyframeTime(base.Firetime, firetime);
            base.Firetime = firetime;

            updateKeyframeTime(base.Firetime + base.Duration, base.Firetime + duration);
            base.Duration = duration;
        }

        public void AlterDuration(float duration)
        {
            updateKeyframeTime(base.Firetime + base.Duration, base.Firetime + duration);
            base.Duration = duration;
        }
    }
}
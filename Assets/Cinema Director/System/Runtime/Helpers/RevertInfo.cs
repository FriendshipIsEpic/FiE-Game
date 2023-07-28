// Cinema Suite
using System;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector.Helpers
{
    /// <summary>
    /// Holds info related to reverting objects to a former state.
    /// </summary>
    public class RevertInfo
    {
        private MonoBehaviour MonoBehaviour;
        private Type Type;
        private object Instance;
        private MemberInfo[] MemberInfo;
        private object value;

        /// <summary>
        /// Set up a revert info for a static object.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour that is making this RevertInfo.</param>
        /// <param name="type">The type of the static object</param>
        /// <param name="memberName">The member name of the field/property/method to be called on revert.</param>
        /// <param name="value">The current value you want to save.</param>
        public RevertInfo(MonoBehaviour monoBehaviour, Type type, string memberName, object value)
        {
            this.MonoBehaviour = monoBehaviour;
            this.Type = type;
            this.value = value;
            this.MemberInfo = Type.GetMember(memberName);
        }

        /// <summary>
        /// Set up Revert Info for an instance object.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour that is making this RevertInfo.</param>
        /// <param name="obj">The instance of the object you want to save.</param>
        /// <param name="memberName">The member name of the field/property/method to be called on revert.</param>
        /// <param name="value">The current value you want to save.</param>
        public RevertInfo(MonoBehaviour monoBehaviour, object obj, string memberName, object value)
        {
            this.MonoBehaviour = monoBehaviour;
            this.Instance = obj;
            this.Type = obj.GetType();
            this.value = value;
            this.MemberInfo = Type.GetMember(memberName);
        }

        /// <summary>
        /// Revert the given object to its former state.
        /// </summary>
        public void Revert()
        {
            if (MemberInfo != null && MemberInfo.Length > 0)
            {
                if (MemberInfo[0] is FieldInfo)
                {
                    FieldInfo fi = (MemberInfo[0] as FieldInfo);
                    if (fi.IsStatic || (!fi.IsStatic && Instance != null))
                    {
                        fi.SetValue(Instance, value);
                    }
                }
                else if (MemberInfo[0] is PropertyInfo)
                {
                    PropertyInfo pi = (MemberInfo[0] as PropertyInfo);
                    //if (Instance != null)
                    {
                        
                        pi.SetValue(Instance, value, null);
                    }
                }
                else if (MemberInfo[0] is MethodInfo)
                {
                    MethodInfo mi = (MemberInfo[0] as MethodInfo);
                    if (mi.IsStatic || (!mi.IsStatic && Instance != null))
                    {
                        object[] values = new object[] { value };
                        mi.Invoke(Instance, values);
                    }
                }
            }
        }

        /// <summary>
        /// Should we apply this revert in runtime.
        /// </summary>
        public RevertMode RuntimeRevert
        {
            get
            {
                return (MonoBehaviour as IRevertable).RuntimeRevertMode;
            }
        }

        /// <summary>
        /// Should we apply this revert in the editor.
        /// </summary>
        public RevertMode EditorRevert
        {
            get
            {
                return (MonoBehaviour as IRevertable).EditorRevertMode;
            }
        }
    }
}

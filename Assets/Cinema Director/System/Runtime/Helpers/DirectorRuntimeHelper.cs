// Cinema Suite 2014

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A helper class for getting useful data from Director Runtime objects.
    /// </summary>
    public static class DirectorRuntimeHelper
    {
        /// <summary>
        /// Returns a list of Track types that are associated with the given Track Group.
        /// </summary>
        /// <param name="trackGroup">The track group to be inspected</param>
        /// <returns>A list of track types that meet the genre criteria of the given track group.</returns>
        public static List<Type> GetAllowedTrackTypes(TrackGroup trackGroup)
        {
            // Get all the allowed Genres for this track group
            TimelineTrackGenre[] genres = new TimelineTrackGenre[0];
            MemberInfo info = trackGroup.GetType();
            foreach (TrackGroupAttribute attribute in info.GetCustomAttributes(typeof(TrackGroupAttribute), true))
            {
                if (attribute != null)
                {
                    genres = attribute.AllowedTrackGenres;
                    break;
                }
            }

            List<Type> allowedTrackTypes = new List<Type>();
            foreach (Type type in DirectorRuntimeHelper.GetAllSubTypes(typeof(TimelineTrack)))
            {
                foreach (TimelineTrackAttribute attribute in type.GetCustomAttributes(typeof(TimelineTrackAttribute), true))
                {
                    if (attribute != null)
                    {
                        foreach (TimelineTrackGenre genre in attribute.TrackGenres)
                        {
                            foreach (TimelineTrackGenre genre2 in genres)
                            {
                                if (genre == genre2)
                                {
                                    allowedTrackTypes.Add(type);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            return allowedTrackTypes;
        }

        /// <summary>
        /// Returns a list of Cutscene Item types that are associated with the given Track.
        /// </summary>
        /// <param name="timelineTrack">The track to look up.</param>
        /// <returns>A list of valid cutscene item types.</returns>
        public static List<Type> GetAllowedItemTypes(TimelineTrack timelineTrack)
        {
            // Get all the allowed Genres for this track
            CutsceneItemGenre[] genres = new CutsceneItemGenre[0];
            MemberInfo info = timelineTrack.GetType();

            foreach (TimelineTrackAttribute attribute in info.GetCustomAttributes(typeof(TimelineTrackAttribute), true))
            {
                if (attribute != null)
                {
                    genres = attribute.AllowedItemGenres;
                    break;
                }
            }

            List<Type> allowedItemTypes = new List<Type>();
            foreach (Type type in DirectorRuntimeHelper.GetAllSubTypes(typeof(TimelineItem)))
            {
                foreach (CutsceneItemAttribute attribute in type.GetCustomAttributes(typeof(CutsceneItemAttribute), true))
                {
                    if (attribute != null)
                    {
                        foreach (CutsceneItemGenre genre in attribute.Genres)
                        {
                            foreach (CutsceneItemGenre genre2 in genres)
                            {
                                if (genre == genre2)
                                {
                                    allowedItemTypes.Add(type);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            return allowedItemTypes;
        }

        /// <summary>
        /// Get all Sub types from the given parent type.
        /// </summary>
        /// <param name="ParentType">The parent type</param>
        /// <returns>all children types of the parent.</returns>
        private static Type[] GetAllSubTypes(System.Type ParentType)
        {
            List<System.Type> list = new List<System.Type>();
            foreach (Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in a.GetTypes())
                {
                    if (type.IsSubclassOf(ParentType))
                    {
                        list.Add(type);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Retrieve all children of a parent Transform recursively.
        /// </summary>
        /// <param name="parent">The parent transform</param>
        /// <returns>All children of that parent.</returns>
        public static List<Transform> GetAllTransformsInHierarchy(Transform parent)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform child in parent)
            {
                children.AddRange(GetAllTransformsInHierarchy(child));
                children.Add(child);
            }
            return children;
        }

    }
}

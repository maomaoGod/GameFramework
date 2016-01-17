﻿//----------------------------------------------
// Flip Web Apps: Game Framework
// Copyright © 2016 Flip Web Apps / Mark Hewitt
//----------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DB = UnityEngine.Debug;

namespace FlipWebApps.GameFramework.Scripts.Editor {
    /// <summary>
    /// Various helper options for selecting items with in the current scene
    /// </summary>
    public class SelectItems : MonoBehaviour {

        /// <summary>
        /// select all items within the numbered layer
        /// </summary>
        /// <param name="layerNum"></param>
        static void SelectLayer(int layerNum) {
            var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep);

            var list = new List<Object>(objs.Length);

            foreach (var obj in objs) {
                var go = obj as GameObject;

                if (go == null) continue;

                if (go.layer == layerNum)
                {
                    list.Add(go);
                }

                Selection.objects = list.ToArray();
            }

            var layerName = LayerMask.LayerToName(layerNum);
            DB.Log(string.Format("Found {0} objects in layer \"{1}\"", list.Count, string.IsNullOrEmpty(layerName) ? layerNum.ToString() : layerName));
        }


        /// <summary>
        /// select all items visible from teh main camera
        /// </summary>
        /// <param name="visible"></param>
        static void SelectVisibleFromMainCamera(bool visible)
        {
            var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep);

            var list = new List<Object>(objs.Length);

            foreach (var obj in objs)
            {
                var go = obj as GameObject;

                if (go == null) continue;

                if (go.GetComponent<Renderer>() != null)
                {
                    if (IsVisibleFrom(go.GetComponent<Renderer>(), Camera.main) == visible)
                        list.Add(go);
                }

                Selection.objects = list.ToArray();
            }

            DB.Log(string.Format("Found {0} objects", list.Count));
        }

        public static bool IsVisibleFrom(Renderer renderer, Camera camera)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }


        /// <summary>
        /// Finds and optionally delete all colliders from the selected gameobjects
        /// </summary>
        /// <param name="remove"></param>
        static void FindPhysicsComponentsFromSelected(bool remove)
        {
            var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep);

            var list = new List<Object>(objs.Length);

            foreach (var obj in objs)
            {
                var go = obj as GameObject;

                if (go == null) continue;

                if (go.GetComponent<Collider>() != null)
                {
                    list.Add(go);
                    if (remove)
                    {
                        DestroyImmediate(go.GetComponent<Collider>());
                    }
                }

            }

            if (!remove)
                Selection.objects = list.ToArray();

            DB.Log(string.Format("Found {0} objects", list.Count));
        }


        /// <summary>
        /// Select all game objects that are empty and don't have any components
        /// </summary>
        static void SelectEmptyGameObjects() //bool remove)
        {
            var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep);

            var list = new List<Object>(objs.Length);

            foreach (var obj in objs)
            {
                var go = obj as GameObject;

                if (go == null) continue;

                if (go.GetComponents(typeof(Component)).Length <= 1 && go.transform.childCount == 0)
                {
                    list.Add(go);
                }

            }

            Selection.objects = list.ToArray();

            DB.Log(string.Format("Found {0} objects", list.Count));
        }


        #region Menu items
        [MenuItem("Window/Flip Web Apps/Select Items/Visible From Camera/Visible")]
        static void SelectVisibleFromMainCamera0()
        {
            SelectVisibleFromMainCamera(true);
        }
        [MenuItem("Window/Flip Web Apps/Select Items/Visible From Camera/Not Visible")]
        static void SelectVisibleFromMainCamera1()
        {
            SelectVisibleFromMainCamera(false);
        }


        [MenuItem("Window/Flip Web Apps/Select Items/Select Colliders")]
        static void RemovePhysicsComponents0() {
            FindPhysicsComponentsFromSelected(false);
        }

        [MenuItem("Window/Flip Web Apps/Select Items/Remove Colliders")]
        static void RemovePhysicsComponents1()
        {
            FindPhysicsComponentsFromSelected(true);
        }

        [MenuItem("Window/Flip Web Apps/Select Items/Select Empty")]
        static void SelectEmptyGameObjects0()
        {
            SelectEmptyGameObjects();
        }

        [MenuItem("Window/Flip Web Apps/Select Items/With Layer/0")]
        static void SelectLayer0()
        {
            SelectLayer(0);
        }

        [MenuItem("Window/Flip Web Apps/Select Items/With Layer/1")]
        static void SelectLayer1()
        {
            SelectLayer(1);
        }

        #endregion
    }
}
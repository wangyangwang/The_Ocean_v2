using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Gamestrap
{
    public class EditorCameraViewsWindow : EditorWindow
    {
        #region Constant Variables
        private const string shortcutPath = "Edit/Gamestrap Editor Camera States/";
        private const string gameObjectDAOName = "Editor Camera Views Data";
        private const int maxSlots = 50;
        private const int maxColumn = 10;
        #endregion

        public static EditorCameraViewsWindow Instance;

        #region Editor variables

        private bool settingsVisible = false;

        private Camera selectedCamera;

        // Scrolling window
        private Vector2 scrollPosition;

        // If the editor should follow the scene camera
        private bool followCamera = false;

        // Reference to the gameobject that it's created to save the specific editor camera states in the scene
        private CameraViewsData cameraViewsData;

        // Selected game state
        private CameraState cameraStateSelected;

        private CameraState lastState = new CameraState();

        #endregion

        #region Show Editor window methods
        [MenuItem("Window/Gamestrap Editor Camera")]
        public static void ShowWindow()
        {
            EditorCameraViewsWindow gs = (EditorCameraViewsWindow)EditorWindow.GetWindow(typeof(EditorCameraViewsWindow), false, "Editor Camera");
            gs.minSize = new Vector2(240f, 110f);
            gs.autoRepaintOnSceneChange = true;
            Instance = gs;
        }

        /// <summary>
        /// Used for shortcut purposes to either save a new state or activate the state in the editor
        /// </summary>
        /// <param name="slotNumber">Index of the slot you want to activate</param>
        private static void ShowView(int slotNumber)
        {
            if (EditorCameraViewsWindow.Instance == null)
            {
                ShowWindow();
            }
            EditorCameraViewsWindow.Instance.ActivateCameraSlot(slotNumber);
            Instance.Repaint();
        }
        #endregion

        #region Editor Window Methods

        void Update()
        {
            if (followCamera)
            {
                if (selectedCamera == null)
                {
                    followCamera = false;
                    return;
                }

                SceneView view = GetSceneView();
                if (view)
                {
                    view.LookAtDirect(selectedCamera.transform.position, selectedCamera.transform.rotation, 0.1f);
                    view.camera.fieldOfView = selectedCamera.fieldOfView;
                    view.orthographic = selectedCamera.orthographic;
                }
            }

        }

        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            RefreshCameraViews();
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            cameraViewsData.CleanSlotCount();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.MouseUp && cameraStateSelected != null)// && CheckView(cameraStateSelected))
            {
                cameraStateSelected = null;
                if (Instance != null)
                    Instance.Repaint();
            }
        }

        void OnHierarchyChange()
        {
            RefreshCameraViews();
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Check to see if there are enough spaces in the list
            if (cameraViewsData.slotCount != cameraViewsData.slots.Count)
            {
                RefreshCameraViews();
            }

            #region Matrix creation of the slot buttons
            GUILayout.BeginHorizontal();
            Color defaultBg = GUI.backgroundColor;

            bool anyStateSelected = false;
            for (int i = 0; i < cameraViewsData.slotCount; i++)
            {
                if (cameraViewsData.slots.Count < cameraViewsData.slotCount)
                {
                    break;
                }
                // Just in case it's more than 5 slots, it will make rows of 5 slots.
                if (i > 0 && i % cameraViewsData.columnCount == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

                CameraState cameraState = cameraViewsData.slots[i];

                // Checks if the slot exists
                if (cameraState.active)
                {
                    GUI.backgroundColor = cameraState.color;
                    // If the slot is selected then it changes the font style
                    if (cameraState == cameraStateSelected)
                    {
                        GUI.skin.button.fontStyle = FontStyle.Bold;
                    }
                    else
                    {
                        GUI.skin.button.fontStyle = FontStyle.Normal;
                        GUI.backgroundColor = cameraState.color;
                    }
                }
                if (cameraStateSelected == cameraState)
                {
                    GUI.skin.button.normal.textColor = Color.white;
                }
                if (GUILayout.Button((cameraState.active) ? cameraState.name : "", GUILayout.MinWidth((position.width - 50f) / cameraViewsData.columnCount)))
                {
                    if (Event.current.button == 0)
                    {
                        if (cameraStateSelected == cameraState)
                        {
                            cameraStateSelected = null;
                            if (lastState != null)
                            {
                                SetView(lastState);
                            }
                        }
                        else {
                            if (cameraStateSelected == null)
                            {
                                //Remembers last state
                                AssignViewValues(lastState);
                            }                    
                            // If clicked an empty one then check anyState selected to later on disable the camera follow.
                            if (ActivateCameraSlot(i))
                            {
                                anyStateSelected = true;
                            }
                            cameraStateSelected = cameraState;
                        }
                    }
                    else if (Event.current.button == 1)
                    {
                        cameraState.active = false;
                        cameraState.color = CameraState.defaultColor;
                        cameraStateSelected = null;
                    }
                }
                GUI.skin.button.normal.textColor = Color.black;
                GUI.skin.button.fontStyle = FontStyle.Normal;
                GUI.backgroundColor = defaultBg;
            }
            GUILayout.EndHorizontal();
            #endregion

            #region Slot number and Detail Slot Data
            GUILayout.BeginHorizontal();
            if (cameraStateSelected == null)
            {
                GUI.enabled = false;
                GUILayout.TextField("Select a slot");
            }
            else
            {
                GUILayout.Label("Slot Name", GUILayout.ExpandWidth(false));
                cameraStateSelected.name = GUILayout.TextField(cameraStateSelected.name);
                cameraStateSelected.color = EditorGUILayout.ColorField(cameraStateSelected.color, GUILayout.MaxWidth(50f));
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            #endregion
      
            #region Camera Follow button

            GUILayout.BeginHorizontal();
            bool fCamera = GUILayout.Toggle(followCamera, "Follow Camera", "Button");

            if (GUILayout.Button("Align With View"))
            {
                bool foundCamera = false;
                SceneView view = GetSceneView();
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go.GetComponent<Camera>())
                    {
                        Camera camera = go.GetComponent<Camera>();
                        foundCamera = true;
                        AlignCameraToViewport(view, camera);
                    }
                }
                if (!foundCamera && Camera.main != null)
                {
                    AlignCameraToViewport(view, Camera.main);
                }
            }

            GUILayout.EndHorizontal();

            if (fCamera != followCamera && fCamera)
            {
                cameraStateSelected = null;
                bool foundCamera = false;
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go.GetComponent<Camera>())
                    {
                        selectedCamera = go.GetComponent<Camera>();
                        foundCamera = true;
                        break;
                    }
                }
                if (!foundCamera && Camera.main != null)
                {
                    selectedCamera = Camera.main;
                }

                if (foundCamera)
                {
                    SceneView view = GetSceneView();
                    AlignViewportToCamera(view, selectedCamera);
                }
                else
                {
                    followCamera = false;
                }
            }
            followCamera = fCamera;

            if (anyStateSelected && followCamera)
            {
                followCamera = false;
            }
            #endregion

            #region Settings

            settingsVisible = EditorGUILayout.Foldout(settingsVisible, "Settings");
            if (settingsVisible)
            {
                EditorGUILayout.BeginHorizontal("Box");
                float lWith = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60f;
                cameraViewsData.slotCount = Mathf.Clamp(EditorGUILayout.IntField("Slots", cameraViewsData.slotCount, GUILayout.MinWidth(30f)), 1, maxSlots);
                cameraViewsData.columnCount = Mathf.Clamp(EditorGUILayout.IntField("Columns", cameraViewsData.columnCount, GUILayout.MinWidth(30f)), 1, maxColumn);
                EditorGUIUtility.labelWidth = lWith;
                EditorGUILayout.EndHorizontal();
            }

            #endregion

            EditorGUILayout.EndScrollView();
        }
        #endregion

        public SceneView GetSceneView()
        {
            SceneView view = null;
            if (SceneView.lastActiveSceneView)
            {
                view = SceneView.lastActiveSceneView;
            } else if (SceneView.sceneViews.Count > 0)
            {
                view = (SceneView)SceneView.sceneViews[0];
            }
            //view.Focus();
            return view;
        }

        #region Camera/Viewport Alignments
        public void AlignCameraToViewport(SceneView view, Camera camera)
        {
            Undo.RecordObject(camera, "Values changed");
            Undo.RecordObject(camera.transform, "Movement");
            camera.fieldOfView = view.camera.fieldOfView;
            camera.orthographic = view.camera.orthographic;
            camera.transform.position = view.camera.transform.position;
            camera.transform.rotation = view.camera.transform.rotation;
            EditorUtility.SetDirty(camera);
            EditorUtility.SetDirty(camera.transform);
        }

        public void AlignViewportToCamera(SceneView view, Camera camera)
        {
            view.camera.fieldOfView = camera.fieldOfView;
            view.orthographic = camera.orthographic;
            view.LookAtDirect(camera.transform.position, camera.transform.rotation, 0.1f);
        }
        #endregion

        #region Helper methods for the camera slot management
        private bool ActivateCameraSlot(int index)
        {
            if (index >= cameraViewsData.slots.Count)
            {
                Debug.LogWarning("Editor camera slot " + index + " doesn't exits");
                return false;
            }

            SceneView view = GetSceneView();
            if (view == null)
            {
                Debug.LogError("There is no Scene Window opened, please open one under 'Window/Scene' to use this function");
                return false;
            }
            CameraState slot = cameraViewsData.slots[index];

            // If the slot is not active then save all of the current editor camera state
            if (!slot.active)
            {
                AssignViewValues(slot, view);
                return false;
            }
            else
            {
                SetView(slot, view);
                cameraStateSelected = slot;
                return true;
            }
        }

        private void AssignViewValues(CameraState slot, SceneView view = null)
        {
            if (view == null)
            {
                view = GetSceneView();
            }
            slot.active = true;
            slot.orthographic = view.camera.orthographic;
            slot.fieldOfView = view.camera.fieldOfView;
            slot.in2dMode = view.in2DMode;
            slot.position = view.camera.transform.position;
            slot.rotation = view.camera.transform.rotation;
            slot.pivot = view.pivot;
            slot.size = view.size;
            slot.renderMode = view.renderMode;
        }

        private void SetView(CameraState slot, SceneView view = null)
        {
            if (view == null)
            {
                view = GetSceneView();
            }
            view.camera.fieldOfView = slot.fieldOfView;
            view.in2DMode = slot.in2dMode;
            view.orthographic = slot.orthographic;
            GameObject g = new GameObject();
            g.transform.position = slot.position;
            g.transform.rotation = slot.rotation;
            view.AlignViewToObject(g.transform);
            view.pivot = slot.pivot;
            view.size = slot.size;
            view.renderMode = slot.renderMode;
            DestroyImmediate(g);
        }

        private bool CheckView(CameraState slot, SceneView view = null)
        {
            if (view == null)
            {
                view = GetSceneView();
            }
            return slot.position == view.camera.transform.position;
        }

        private void RefreshCameraViews()
        {
            GameObject cameraViews = GameObject.Find(gameObjectDAOName);

            if (cameraViews == null || cameraViews.GetComponent<CameraViewsData>() == null)
            {
                cameraViews = new GameObject(gameObjectDAOName);
                cameraViews.tag = "EditorOnly";
                cameraViewsData = cameraViews.AddComponent<CameraViewsData>();
                cameraViews.hideFlags = HideFlags.HideInHierarchy; // This hides the gameobject that handles the scene editor camera state
            }
            else
            {
                cameraViewsData = cameraViews.GetComponent<CameraViewsData>();
                //cameraViews.hideFlags = HideFlags.None; // If you want to show the gameobject in charge of the data in the scene
            }
            if (cameraViewsData.slots == null)
            {
                cameraViewsData.slots = new List<CameraState>();
            }
            cameraViewsData.UpdateSlotCount();
        }
        #endregion

        #region Shortcut Menu items
        [MenuItem(shortcutPath + "1 &1")]
        static void ActivateSlot00()
        {
            ShowView(0);
        }

        [MenuItem(shortcutPath + "2 &2")]
        static void ActivateSlot01()
        {
            ShowView(1);
        }

        [MenuItem(shortcutPath + "3 &3")]
        static void ActivateSlot02()
        {
            ShowView(2);
        }

        [MenuItem(shortcutPath + "4 &4")]
        static void ActivateSlot03()
        {
            ShowView(3);
        }

        [MenuItem(shortcutPath + "5 &5")]
        static void ActivateSlot04()
        {
            ShowView(4);
        }
        #endregion

    }
}
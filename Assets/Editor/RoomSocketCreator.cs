using UnityEditor;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Editor
{
    public class RoomSocketCreator : EditorWindow
    {
        [MenuItem("Tools/9x9/Create Room Sockets")]
        public static void ShowWindow()
        {
            GetWindow<RoomSocketCreator>("Room Socket Creator");
        }

        private float m_RoomSize = 10f;
        private bool m_CreateFloorSocket = true;
        private bool m_CreateCeilingSocket = true;
        private bool m_CreateWallSockets = true;

        private void OnGUI()
        {
            GUILayout.Label("Room Socket Creator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Creates socket transforms on the selected GameObject for RoomAssembler.", MessageType.Info);

            m_RoomSize = EditorGUILayout.FloatField("Room Size", m_RoomSize);
            m_CreateFloorSocket = EditorGUILayout.Toggle("Create Floor Socket", m_CreateFloorSocket);
            m_CreateCeilingSocket = EditorGUILayout.Toggle("Create Ceiling Socket", m_CreateCeilingSocket);
            m_CreateWallSockets = EditorGUILayout.Toggle("Create Wall Sockets", m_CreateWallSockets);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Sockets on Selected"))
            {
                CreateSockets();
            }
        }

        private void CreateSockets()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select a GameObject in the hierarchy.", "OK");
                return;
            }

            Undo.RegisterCompleteObjectUndo(selected, "Create Room Sockets");

            float halfSize = m_RoomSize / 2f;

            // Floor socket (bottom center)
            if (m_CreateFloorSocket)
            {
                CreateSocket(selected, "socket_floor", new Vector3(0, -halfSize, 0), Quaternion.identity);
            }

            // Ceiling socket (top center)
            if (m_CreateCeilingSocket)
            {
                CreateSocket(selected, "socket_ceiling", new Vector3(0, halfSize, 0), Quaternion.identity);
            }

            // Wall sockets (centered on each face, forward pointing outward)
            if (m_CreateWallSockets)
            {
                CreateSocket(selected, "socket_north", new Vector3(0, 0, halfSize), Quaternion.Euler(0, 0, 0));
                CreateSocket(selected, "socket_south", new Vector3(0, 0, -halfSize), Quaternion.Euler(0, 180, 0));
                CreateSocket(selected, "socket_east", new Vector3(halfSize, 0, 0), Quaternion.Euler(0, 90, 0));
                CreateSocket(selected, "socket_west", new Vector3(-halfSize, 0, 0), Quaternion.Euler(0, -90, 0));
            }

            EditorUtility.DisplayDialog("Sockets Created", $"Created sockets on {selected.name}.", "OK");
        }

        private void CreateSocket(GameObject parent, string name, Vector3 localPosition, Quaternion localRotation)
        {
            // Check if socket already exists
            Transform existing = parent.transform.Find(name);
            if (existing != null)
            {
                Debug.LogWarning($"Socket '{name}' already exists on {parent.name}. Skipping.");
                return;
            }

            GameObject socket = new GameObject(name);
            socket.transform.SetParent(parent.transform);
            socket.transform.localPosition = localPosition;
            socket.transform.localRotation = localRotation;
            socket.transform.localScale = Vector3.one;

            Debug.Log($"Created socket '{name}' at {localPosition}");
        }
    }
}

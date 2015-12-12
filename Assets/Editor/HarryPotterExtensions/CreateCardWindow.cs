using System.IO;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
using HarryPotterUnity.DeckGeneration.Requirements;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HarryPotterExtensions
{
    [UsedImplicitly]
    public class CreateCardWindow : EditorWindow 
    {
        private struct CreateCardRequest
        {
            public string CardName { get; set; }
            public Texture2D CardGraphic { get; set; }

            public Type CardType { get; set; }
            public ClassificationTypes Classification { get; set; }

            public bool AddLessonRequirement { get; set; }
            public LessonTypes LessonType { get; set; }
            public int LessonAmtRequired { get; set; }
            
            public bool AddCardLimit { get; set; }
            public int MaxAllowedInDeck { get; set; }
            
            public bool IsValid
            {
                get
                {
                    return CardGraphic != null && !string.IsNullOrEmpty(CardName);
                }
            }
        }

        private CreateCardRequest _cardRequest;


        [MenuItem("Harry Potter TCG/Add New Card"), UsedImplicitly]
        public static void AddCard()
        {
            GetWindow(typeof(CreateCardWindow), true, "New Card");
        }
        
        private static void CreateCard(CreateCardRequest request)
        {
            GameObject card = InstantiateCardTemplate();
            card.transform.name = request.CardName;

            Material newMaterial = CreateNewMaterial(request);
            card.transform.FindChild("Front").gameObject.GetComponent<Renderer>().material = newMaterial;

            if (request.AddLessonRequirement)
            {
                var newComponent = card.AddComponent<LessonRequirement>();
                newComponent.AmountRequired = request.LessonAmtRequired;
                newComponent.LessonType = request.LessonType;
            }

            if (request.AddCardLimit)
            {
                var newComponent = card.AddComponent<DeckCardLimitRequirement>();
                newComponent.MaximumAmountAllowed = request.MaxAllowedInDeck;
            }
            
            //TODO: Create script: http://answers.unity3d.com/questions/12599/editor-script-need-to-create-class-script-automati.html
            //TODO: Create prefab and focus project window
            //Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/"+prefabName+".prefab");
            //EditorUtility.FocusProjectWindow();

            string newPrefabAssetPath = string.Format("Assets/Prefabs/Resources/Cards/{0}/{1}s/{2}.prefab", 
                request.Classification, 
                request.CardType,
                request.CardName);

            if (AssetDatabase.LoadAssetAtPath(newPrefabAssetPath, typeof (GameObject)))
            {
                string message = string.Format("A prefab already exists at\n{0}\nReplace?", newPrefabAssetPath);

                if (EditorUtility.DisplayDialog("Prefab already exists", message, "Yes", "No"))
                {
                    CreatePrefab(newPrefabAssetPath, card);
                }
            }
            else
            {
                CreatePrefab(newPrefabAssetPath, card);
            }
        }

        private static void CreatePrefab(string newPrefabAssetPath, GameObject card)
        {
            Object empty = PrefabUtility.CreateEmptyPrefab(newPrefabAssetPath);
            card = PrefabUtility.ReplacePrefab(card, empty);

            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(newPrefabAssetPath);
            EditorGUIUtility.PingObject(card);
            EditorUtility.FocusProjectWindow();
        }

        private static Material CreateNewMaterial(CreateCardRequest request)
        {
            string newMaterialAssetPath = string.Format("Assets/Materials/{0}s/{1}/{2}Mat.mat",
                request.CardType,
                request.Classification,
                request.CardName);

            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = request.CardGraphic
            };
            
            if (AssetDatabase.GetAllAssetPaths().Contains(newMaterialAssetPath) == false)
            {
                var path = Path.GetDirectoryName(newMaterialAssetPath);
                if (path != null && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                AssetDatabase.CreateAsset(material, newMaterialAssetPath);
            }
            else
            {
                Debug.LogError(string.Format("Asset at {0} already exists!", newMaterialAssetPath));
                return null;
            }
            
            return material;
        }

        private static GameObject InstantiateCardTemplate()
        {
            string templatePath =
                AssetDatabase.GetAllAssetPaths().Single(path => path.Contains("CardTemplate.prefab"));

            var template = AssetDatabase.LoadAssetAtPath(templatePath, typeof (GameObject)) as GameObject;

            return (GameObject) Instantiate(template, Vector3.zero, Quaternion.identity);
        }

        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Create New Card", EditorStyles.boldLabel);

            ChooseCardTexture();
            ChooseCardProperties();
            ChooseAddLessonRequirement();
            ChooseAddCardLimitRequirement();
            ValidateRequest();

            GUILayout.EndVertical();
        }

        private void ValidateRequest()
        {
            GUILayout.Space(10);
            if (!_cardRequest.IsValid) return;

            if (GUILayout.Button("Create Card"))
            {
                CreateCard(_cardRequest);
                Close();
            }
        }

        private void ChooseAddCardLimitRequirement()
        {
            GUILayout.Space(10);

            _cardRequest.AddCardLimit = EditorGUILayout.BeginToggleGroup("Add Card Limit", _cardRequest.AddCardLimit);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Allowed In Deck: ");
            _cardRequest.MaxAllowedInDeck = EditorGUILayout.IntField(_cardRequest.MaxAllowedInDeck);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();
        }

        private void ChooseAddLessonRequirement()
        {
            GUILayout.Space(10);

            _cardRequest.AddLessonRequirement = EditorGUILayout.BeginToggleGroup("Add Lesson Requirement", _cardRequest.AddLessonRequirement);
            
            _cardRequest.LessonType = (LessonTypes) EditorGUILayout.EnumPopup("Lesson Type: ", _cardRequest.LessonType);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Amount Required: ");
            _cardRequest.LessonAmtRequired = EditorGUILayout.IntField(_cardRequest.LessonAmtRequired);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();
        }

        private void ChooseCardProperties()
        {
            GUILayout.Space(10);

            _cardRequest.CardType = (Type) EditorGUILayout.EnumPopup("Card Type: ", _cardRequest.CardType);
            _cardRequest.Classification = (ClassificationTypes) EditorGUILayout.EnumPopup("Classification: ", _cardRequest.Classification);
        }

        private void ChooseCardTexture()
        {
            GUILayout.Space(10);

            _cardRequest.CardGraphic = (Texture2D) EditorGUILayout.ObjectField("Card Image:", _cardRequest.CardGraphic, typeof (Texture2D), false);
            if (_cardRequest.CardGraphic != null)
            {
                _cardRequest.CardName = _cardRequest.CardGraphic.name.Replace("_", "").Replace("'", "");
            }
        }
    }
}

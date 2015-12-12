using System.IO;
using System.Linq;
using HarryPotterUnity.Cards.PlayRequirements;
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
            public string Cardname { get; set; }
            public Texture2D CardGraphic { get; set; }

            public Type CardType { get; set; }
            public ClassificationTypes Classification { get; set; }

            public bool AddLessonRequirement { get; set; }
            public LessonTypes LessonType { get; set; }
            public int LessonAmtRequired { get; set; }

            public bool AddScript { get; set; }
            public string ScriptName { get; set; }
        }
        

        [MenuItem("Harry Potter TCG/Add New Card"), UsedImplicitly]
        public static void AddCard()
        {
            GetWindow(typeof(CreateCardWindow), true, "New Card");
        }

        private CreateCardRequest _cardRequest;
        

        private static void CreateCard(CreateCardRequest request)
        {
            GameObject card = InstantiateCardTemplate();

            Material newMaterial = CreateNewMaterial(request);
            card.transform.FindChild("Front").gameObject.GetComponent<Renderer>().material = newMaterial;

            if (request.AddLessonRequirement)
            {
                var requirement = new LessonRequirement()
                {
                    AmountRequired = request.LessonAmtRequired,
                    LessonType = request.LessonType
                };

                var newComponent = card.AddComponent<LessonRequirement>();
                newComponent.AmountRequired = requirement.AmountRequired;
                newComponent.LessonType = requirement.LessonType;
            }

            //TODO: maybe don't need this...
            if (request.AddScript)
            {
                
            }
        }

        private static Material CreateNewMaterial(CreateCardRequest request)
        {
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = request.CardGraphic
            };

            string newMaterialAssetPath = string.Format("Assets/Materials/{0}s/{1}/{2}Mat.mat", 
                request.CardType, 
                request.Classification,
                request.Cardname);
            
            //TODO: Check if asset exists
            AssetDatabase.CreateAsset(material, newMaterialAssetPath);
            
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
            ChooseAddCustomScript();

            GUILayout.Space(10);

            if (GUILayout.Button("Create Card"))
            {
                //TODO: Validate Input
                CreateCard(_cardRequest);
                Close();
            }

            GUILayout.EndVertical();
        }

        private void ChooseAddCustomScript()
        {
            GUILayout.Space(10);

            _cardRequest.AddScript = GUILayout.Toggle(_cardRequest.AddScript, "Add Custom Script");
            if (!_cardRequest.AddScript) return;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Script Name: ");
            _cardRequest.ScriptName = EditorGUILayout.TextField(_cardRequest.ScriptName);
            GUILayout.EndHorizontal();
        }

        private void ChooseAddLessonRequirement()
        {
            GUILayout.Space(10);

            _cardRequest.AddLessonRequirement = GUILayout.Toggle(_cardRequest.AddLessonRequirement, "Add Lesson Requirement");
            if (!_cardRequest.AddLessonRequirement) return;

            _cardRequest.LessonType = (LessonTypes) EditorGUILayout.EnumPopup("Lesson Type: ", _cardRequest.LessonType);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Amount Required: ");
            _cardRequest.LessonAmtRequired = EditorGUILayout.IntField(_cardRequest.LessonAmtRequired);
            GUILayout.EndHorizontal();
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

            //TODO: Write standalone script that rotates all horizontal images in our libarary
            _cardRequest.CardGraphic = (Texture2D) EditorGUILayout.ObjectField("Card Image:", _cardRequest.CardGraphic, typeof (Texture2D), false);
            if (_cardRequest.CardGraphic != null)
            {
                _cardRequest.Cardname = _cardRequest.CardGraphic.name;
            }
        }
    }
}

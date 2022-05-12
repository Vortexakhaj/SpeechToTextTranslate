using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace UniLang
{
    /// <summary>
    /// 翻译
    /// </summary>
    public class Translator : MonoBehaviour
    {
        /// <summary>
        /// google翻译api
        /// </summary>
        //const string k_Url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";

        private const string APIKey = "AIzaSyCbe1VqpibB2V9nWHp3R6tqcpUpW38W9iI";

        static Translator s_instance;

        /// <summary>
        /// 翻译接口
        /// </summary>
        /// <param name="sourceLang">原始语言类型</param>
        /// <param name="targetLang">目标语言类型</param>
        /// <param name="text">要翻译的文字</param>
        /// <param name="callback">翻译回调</param>
        public static void Do(string sourceLang, string targetLang, string text, Action<string> callback)
        {
            if (s_instance == null)
            {
                var obj = new GameObject("Translation");
                //obj.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(obj);
                s_instance = obj.AddComponent<Translator>();
            }
            s_instance.Run(sourceLang, targetLang, text, callback);
        }

        public void Run(string sourceLang, string targetLang, string text, Action<string> callback)
        {
            StartCoroutine(TranslateAsync(sourceLang, targetLang, text, callback));
        }

        IEnumerator TranslateAsync(string sourceLang, string targetLang, string text, Action<string> callback)
        {
            //var requestUrl = String.Format(k_Url, new object[] { sourceLang, targetLang, text });
            //Debug.Log("url: " + requestUrl);
            //UnityWebRequest req = UnityWebRequest.Get(requestUrl);

            var formData = new List<IMultipartFormSection>
            {
            new MultipartFormDataSection("Content-Type", "application/json; charset=utf-8"),
            new MultipartFormDataSection("source", sourceLang),
            new MultipartFormDataSection("target", targetLang),
            new MultipartFormDataSection("format", "text"),
            new MultipartFormDataSection("q", text)
            };

            var uri = $"https://translate.googleapis.com/language/translate/v2?key={APIKey}";
            var req = UnityWebRequest.Post(uri, formData);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(req.error);
                callback.Invoke(string.Empty);

                yield break;
            }

            var parsedTexts = JSONNode.Parse(req.downloadHandler.text);
            var translatedText = parsedTexts["data"]["translations"][0]["translatedText"];

            callback.Invoke(translatedText);


            /*if (string.IsNullOrEmpty(req.error))
            {
                Debug.Log("req" + req.downloadHandler.text);
                JSONArray jsonArray = JSONConvert.DeserializeArray(req.downloadHandler.text);
                jsonArray = (JSONArray)(jsonArray[0]);
                jsonArray = (JSONArray)(jsonArray[0]);
                callback((string)jsonArray[0]);
            }
            else
            {
                callback(req.error);
            }*/
        }
    }
}

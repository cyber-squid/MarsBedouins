//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using DevToDev.Analytics;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public static class DTDCustomEvents
//{
//    private static string GetUniqueUserID => $"{SystemInfo.deviceModel} - {SystemInfo.deviceUniqueIdentifier}";

//    public static void MakeAnalyticsEvent(string eventName, DTDCustomEventParameters eventParams)
//    {
//        Debug.Log($"{eventName} called for dev2dev");
//        eventParams = AppendUserIDHeader(eventParams);

//        eventParams = AppendApplicationVersionHeader(eventParams);

//        DTDAnalytics.CustomEvent(eventName, eventParams);
//#if !UNITY_EDITOR
//                DTDAnalytics.CustomEvent(eventName, eventParams);
//#endif
//    }
        
//    public static void MakeAnalyticsEvent(string eventName) => MakeAnalyticsEvent(eventName, new DTDCustomEventParameters());


//    #region Custom Events

//    public static void StartedGame()
//    {
//        var EventName = MethodBase.GetCurrentMethod().Name;
//        MakeAnalyticsEvent(EventName);
//    }

//    #endregion
    
    
//    #region Headers
        
        
//    public static DTDCustomEventParameters AppendUserIDHeader(DTDCustomEventParameters parameters)
//    {
//        parameters.Add("User", GetUniqueUserID);
//        return parameters;
//    }

//    public static DTDCustomEventParameters AppendApplicationVersionHeader(DTDCustomEventParameters parameters)
//    {
//        parameters.Add("Build Num", Application.version);
//        return parameters;
//    }
        
        
//    #endregion

//}
using UnityEngine;
using UnityEngine.Advertisements;

namespace BrickBreak
{
    public class AdManager : MonoBehaviour
    {
        [SerializeField] string _androidAdUnitId = "4495831";
        [SerializeField] string _iOsAdUnitId = "4495830";
        string _adUnitId;

        public void OnEnable()
        {
            // Get the Ad Unit ID for the current platform:
            _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
                ? _iOsAdUnitId
                : _androidAdUnitId;
        }

        // Load content to the Ad Unit:
        public void InitializeAd()
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _adUnitId);
            Advertisement.Initialize(_adUnitId);
        }

        public void LoadAd()
        {
            Advertisement.Load(_adUnitId);
        }

        // Show the loaded content in the Ad Unit: 
        public void ShowAd()
        {
            if (Advertisement.isInitialized)
            {
                Debug.Log("Showing Ad: " + _adUnitId);
                Advertisement.Show();
                return;

            }
            else
            {
                Debug.Log("Ad not ready: " + _adUnitId);
            }
            
               
            
            // Note that if the ad content wasn't previously loaded, this method will fail
            
        }

        // Implement Load Listener and Show Listener interface methods:  
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("loaded");
            // Optionally execute code if the Ad Unit successfully loads content.
        }



        public void OnUnityAdsShowStart(string adUnitId) { }
        public void OnUnityAdsShowClick(string adUnitId) { }
    }
}

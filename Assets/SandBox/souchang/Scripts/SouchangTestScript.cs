using System;
using TeamB.Scripts.Common.API;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SouchangTestScript : MonoBehaviour
{
    [SerializeField] private Button sampleButton;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Test");
        sampleButton.onClick.AddListener(OnClick);
    }

    private async void OnClick()
    {
        Debug.Log("Click!");

        var geoIp = await APIClient.GetGeoIP();
        Debug.Log("IP: " + geoIp.ip);
        Debug.Log("ASN: " + geoIp.asn);
        Debug.Log("Org: " + geoIp.asn_organization);

        var user = await APIClient.Users.SignUp("tests");
        await APIClient.DoLogin(user.user_id);
        var dict = new Dictionary<string, uint>
        {
            {
                "grimmy", 10
            },
            {
                "fangculite", 10
            },
            {
                "mellogardia", 10
            },
        };

        try
        {
            var voteRankings = await APIClient.ScoreRanking.UpdateScoreRanking(user.user_id, dict);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }
}
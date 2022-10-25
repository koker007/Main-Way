using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamCTRL : MonoBehaviour
{
    static public CSteamID MySteamID; //Мой ID
    static public Texture2D MyIcon;

    public enum IconSize {
        Small,
        Medium,
        Large
    }

    // Start is called before the first frame update
    void Start()
    {
        MySteamID = SteamUser.GetSteamID();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public static Texture2D GetPlayerIcon(CSteamID SteamUserID, IconSize size) {
        Texture2D result = null;

        uint width = 0;
        uint height = 0;
        int avatarInt = 0;
        if (size == IconSize.Small)
        {
            avatarInt = SteamFriends.GetSmallFriendAvatar(SteamUserID);
        }
        else if (size == IconSize.Medium)
        {
            avatarInt = SteamFriends.GetMediumFriendAvatar(SteamUserID);
        }
        else if (size == IconSize.Large)
        {
            avatarInt = SteamFriends.GetLargeFriendAvatar(SteamUserID);
        }

        if (avatarInt > 0)
        {
            SteamUtils.GetImageSize(avatarInt, out width, out height);
        }

        if (width > 0 && height > 0)
        {
            byte[] avatarStream = new byte[4 * (int)width * (int)height];
            SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

            result = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            result.LoadRawTextureData(avatarStream);
            result.Apply();
        }

        return result;
    }
}

using KBEngine;
using UnityEngine;
using System; 
using System.IO;  
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour 
{
	public static UI inst;
	
	public int ui_state = 0;
	private string stringAccount = "";
	private string stringPasswd = "";
	private string labelMsg = "";
	private Color labelColor = Color.green;
	
	private Dictionary<UInt64, AVATAR_INFOS> ui_avatarList = null;
	
	private string stringAvatarName = "";
	private bool startCreateAvatar = false;

	private UInt64 selAvatarDBID = 0;
	public bool showReliveGUI = false;


    public bool showAutoReadyButton = true;
    public bool showManualReadyButton = true;
    public bool showSelectAiming = false;
    public bool showCountDownLabel = false;
    public double shootCountDown = 0;
    public bool toShootManually = true;
    public bool showAllowFireButton = false;
    public bool showBang = false;
    public UnityEngine.GameObject goingToShootAt;
    public bool showDemoNewGame = false;

    bool startRelogin = false;
	
	void Awake() 
	 {
		inst = this;
		DontDestroyOnLoad(transform.gameObject);
	 }
	 
	// Use this for initialization
	void Start () 
	{
		installEvents();
        initialDemoStatus();
        SceneManager.LoadScene("login");
	}

    void initialDemoStatus()
    {
        showAutoReadyButton = true;
        showManualReadyButton = true;
        showSelectAiming = false;
        showCountDownLabel = false;
        shootCountDown = 0;
        toShootManually = true;
        showAllowFireButton = false;
        showBang = false;
        goingToShootAt = null;
        showDemoNewGame = false;
    }

	void installEvents()
	{
		// common
		KBEngine.Event.registerOut("onKicked", this, "onKicked");
		KBEngine.Event.registerOut("onDisconnected", this, "onDisconnected");
		KBEngine.Event.registerOut("onConnectionState", this, "onConnectionState");
		
		// login
		KBEngine.Event.registerOut("onCreateAccountResult", this, "onCreateAccountResult");
		KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
		KBEngine.Event.registerOut("onVersionNotMatch", this, "onVersionNotMatch");
		KBEngine.Event.registerOut("onScriptVersionNotMatch", this, "onScriptVersionNotMatch");
		KBEngine.Event.registerOut("onLoginBaseappFailed", this, "onLoginBaseappFailed");
		KBEngine.Event.registerOut("onLoginSuccessfully", this, "onLoginSuccessfully");
		KBEngine.Event.registerOut("onReloginBaseappFailed", this, "onReloginBaseappFailed");
		KBEngine.Event.registerOut("onReloginBaseappSuccessfully", this, "onReloginBaseappSuccessfully");
		KBEngine.Event.registerOut("onLoginBaseapp", this, "onLoginBaseapp");
		KBEngine.Event.registerOut("Loginapp_importClientMessages", this, "Loginapp_importClientMessages");
		KBEngine.Event.registerOut("Baseapp_importClientMessages", this, "Baseapp_importClientMessages");
		KBEngine.Event.registerOut("Baseapp_importClientEntityDef", this, "Baseapp_importClientEntityDef");
		
		// select-avatars(register by scripts)
		KBEngine.Event.registerOut("onReqAvatarList", this, "onReqAvatarList");
		KBEngine.Event.registerOut("onCreateAvatarResult", this, "onCreateAvatarResult");
		KBEngine.Event.registerOut("onRemoveAvatar", this, "onRemoveAvatar");
	}

	void OnDestroy()
	{
        KBEngine.Event.deregisterOut(this);
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyUp(KeyCode.Space))
        {
			Debug.Log("KeyCode.Space");
			KBEngine.Event.fireIn("jump");
        }

        if (showCountDownLabel)
        {
            if (shootCountDown > 0)
            {
                shootCountDown -= Time.deltaTime;
            }
            else if (shootCountDown == 0)
            {
                if (DateTime.Now.Second % 5 == 0 && DateTime.Now.Millisecond < 100 && !showBang)
                {
                    UI.inst.shootCountDown = 3.8f;
                }
            }
            else
            {
                shootCountDown = 0;
                showCountDownLabel = false;

                if (toShootManually)
                {
                    showAllowFireButton = true;
                }
                else
                {
                    showBang = true;
                    _shotTrigger();
                }

            }
        }
	}

    private void _shotTrigger()
    {
        string[] s = goingToShootAt.name.Split(new char[] { '_' });

        if (s.Length > 0)
        {
            int targetEntityID = Convert.ToInt32(s[s.Length - 1]);
            Write.Log("Shot at someone");
            KBEngine.Event.fireIn("useTargetSkill", (Int32)1, (Int32)targetEntityID);
            showDemoNewGame = true;
        }
    }

	
	void onSelAvatarUI()
	{
		if (startCreateAvatar == false && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 40, 200, 30), "RemoveAvatar"))    
        {
			if(selAvatarDBID == 0)
			{
				err("Please select a Avatar!");
			}
			else
			{
				info("Please wait...");
				
				if(ui_avatarList != null && ui_avatarList.Count > 0)
				{
					AVATAR_INFOS avatarinfo = ui_avatarList[selAvatarDBID];
					KBEngine.Event.fireIn("reqRemoveAvatar", avatarinfo.name);
				}
			}
        }

		if (startCreateAvatar == false && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 75, 200, 30), "CreateAvatar"))    
		{
			startCreateAvatar = !startCreateAvatar;
		}

        if (startCreateAvatar == false && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 110, 200, 30), "EnterGame"))    
        {
        	if(selAvatarDBID == 0)
        	{
        		err("Please select a Avatar!");
        	}
        	else
        	{
        		info("Please wait...");
        		
				KBEngine.Event.fireIn("selectAvatarGame", selAvatarDBID);
				SceneManager.LoadScene("world");
				ui_state = 2;
			}
        }
		
		if(startCreateAvatar)
		{
	        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 40, 200, 30), "CreateAvatar-OK"))    
	        {
	        	if(stringAvatarName.Length > 1)
	        	{
		        	startCreateAvatar = !startCreateAvatar;
					KBEngine.Event.fireIn("reqCreateAvatar", (Byte)1, stringAvatarName);
				}
				else
				{
					err("avatar name is null!");
				}
	        }
	        
	        stringAvatarName = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height - 75, 200, 30), stringAvatarName, 20);
		}
		
		if(ui_avatarList != null && ui_avatarList.Count > 0)
		{
			int idx = 0;
			foreach(UInt64 dbid in ui_avatarList.Keys)
			{
				AVATAR_INFOS info = ui_avatarList[dbid];

				idx++;
				
				Color color = GUI.contentColor;
				if(selAvatarDBID == info.dbid)
				{
					GUI.contentColor = Color.red;
				}
				
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 - 35 * idx, 200, 30), info.name))    
				{
					Debug.Log("selAvatar:" + info.name);
					selAvatarDBID = info.dbid;
				}
				
				GUI.contentColor = color;
			}
		}
		else
		{
			if(KBEngineApp.app.entity_type == "Account")
			{
				KBEngine.Account account = (KBEngine.Account)KBEngineApp.app.player();
				if(account != null)
					ui_avatarList = new Dictionary<UInt64, AVATAR_INFOS>(account.avatars);
			}
		}
	}
	
	void onLoginUI()
	{
		if(GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 30), "Login"))  
        {  
        	Debug.Log("stringAccount:" + stringAccount);
        	Debug.Log("stringPasswd:" + stringPasswd);
        	
			if(stringAccount.Length > 0 && stringPasswd.Length > 5)
			{
                PlayerPrefs.SetString("AccountName", stringAccount);
                PlayerPrefs.SetString("AccountPasswd", stringPasswd);

                login();
			}
			else
			{
				err("account or password is wrong, length < 6!");
			}
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 70, 200, 30), "CreateAccount"))  
        {  
			Debug.Log("stringAccount:" + stringAccount);
			Debug.Log("stringPasswd:" + stringPasswd);

			if(stringAccount.Length > 0 && stringPasswd.Length > 5)
			{
                PlayerPrefs.SetString("AccountName", stringAccount);
                PlayerPrefs.SetString("AccountPasswd", stringPasswd);

                createAccount();
			}
			else
			{
				err("account or password is wrong, length < 6!");
			}
        }

        if (stringAccount == "")
            stringAccount = PlayerPrefs.GetString("AccountName", "");

        if (stringPasswd == "")
            stringPasswd = PlayerPrefs.GetString("AccountPasswd", "");

        stringAccount = GUI.TextField(new Rect (Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 30), stringAccount, 20);
		stringPasswd = GUI.PasswordField(new Rect (Screen.width / 2 - 100, Screen.height / 2 - 10, 200, 30), stringPasswd, '*');
    }

	void onWorldUI()
	{
		if(showReliveGUI)
		{
			if(GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "Relive"))  
			{
				KBEngine.Event.fireIn("relive", (Byte)1);
                initialDemoStatus();	        	
			}
		}

        GUIStyle myStyle = new GUIStyle(GUI.skin.label);
        myStyle.fontSize = 50;
        myStyle.normal.textColor = Color.red;

        if (showDemoNewGame)
        {
            if (GUI.Button(new Rect(Screen.width / 2 + 200, Screen.height / 2 + 200, 200, 30), "Try Again"))
            {
                initialDemoStatus();
            }
        }

        if (showCountDownLabel)
            GUI.Label(new Rect((Screen.width / 2), Screen.height / 2, 200, 100), ((int)shootCountDown).ToString(), myStyle);

        if (showBang)
            GUI.Label(new Rect((Screen.width / 2), Screen.height / 2, 200, 100), "BANG!", myStyle);

        if (showAutoReadyButton)
        {
            if (GUI.Button(new Rect(Screen.width/2 - 400, Screen.height / 2, 200, 30), "Auto shoot"))
            {
                showAutoReadyButton = false;
                showManualReadyButton = false;
                showSelectAiming = true;
                toShootManually = false;
                showCountDownLabel = false;
                //shootCountDown = 0;
                showBang = false;
            }
        }


        if (showManualReadyButton)
        {
            if (GUI.Button(new Rect(Screen.width/2 + 200, Screen.height / 2, 200, 30), "Manual shoot"))
            {
                showAutoReadyButton = false;
                showManualReadyButton = false;
                showSelectAiming = true;
                toShootManually = true;
                showCountDownLabel = false;
                //shootCountDown = 0;
                showBang = false;
            }
        }

        if (showSelectAiming)
            GUI.Label(new Rect((Screen.width / 2 - 150), Screen.height / 2, 400, 100), "Select Enemy", myStyle);

        if (showAllowFireButton)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "FIRE!!"))
            {
                showBang = true;
                showAllowFireButton = false;
                _shotTrigger();
            }
        }


        //if(showStartShootCounting)
        //{
        //    Debug.Log("showStartShootCounting is true");
        //    //UI.inst.showStartShootCounting = true;
        //    GUI.Label(new Rect((Screen.width / 2), Screen.height / 2, 200, 100), ((int)shootCountDown).ToString());
        //}
        //else
        //{
        //    Debug.Log("showStartShootCounting is false");
        //    if (GUI.Button(new Rect(Screen.width - 800, Screen.height / 2, 200, 30), "READY to shoot"))
        //    {
        //        UI.inst.showStartShootCounting = true;
        //    }
        //}

        UnityEngine.GameObject obj = UnityEngine.GameObject.Find("player(Clone)");
		if(obj != null)
		{
			GUI.Label(new Rect((Screen.width / 2) - 100, 20, 400, 100), "id=" + KBEngineApp.app.entity_id + ", position=" + obj.transform.position.ToString()); 
		}
	}

    void OnGUI()  
    {  
		if(ui_state == 1)
		{
			onSelAvatarUI();
   		}
   		else if(ui_state == 2)
   		{
			onWorldUI();
   		}
   		else
   		{
   			onLoginUI();
   		}
   		
		if(KBEngineApp.app != null && KBEngineApp.app.serverVersion != "" 
			&& KBEngineApp.app.serverVersion != KBEngineApp.app.clientVersion)
		{
			labelColor = Color.red;
			labelMsg = "version not match(curr=" + KBEngineApp.app.clientVersion + ", srv=" + KBEngineApp.app.serverVersion + " )(版本不匹配)";
            labelMsg += "\nExecute the gensdk script to generate matching client SDK in the server-assets directory.";
            labelMsg += "\n(在服务端的资产目录下执行gensdk脚本生成匹配的客户端SDK)";
        }
		else if(KBEngineApp.app != null && KBEngineApp.app.serverScriptVersion != "" 
			&& KBEngineApp.app.serverScriptVersion != KBEngineApp.app.clientScriptVersion)
		{
			labelColor = Color.red;
			labelMsg = "scriptVersion not match(curr=" + KBEngineApp.app.clientScriptVersion + ", srv=" + KBEngineApp.app.serverScriptVersion + " )(脚本版本不匹配)";
		}
		
		GUI.contentColor = labelColor;
		GUI.Label(new Rect((Screen.width / 2) - 100, 40, 400, 100), labelMsg);

		GUI.Label(new Rect(0, 5, 400, 100), "client version: " + KBEngine.KBEngineApp.app.clientVersion);
		GUI.Label(new Rect(0, 20, 400, 100), "client script version: " + KBEngine.KBEngineApp.app.clientScriptVersion);
		GUI.Label(new Rect(0, 35, 400, 100), "server version: " + KBEngine.KBEngineApp.app.serverVersion);
		GUI.Label(new Rect(0, 50, 400, 100), "server script version: " + KBEngine.KBEngineApp.app.serverScriptVersion);
	}  
	
	public void err(string s)
	{
		labelColor = Color.red;
		labelMsg = s;
	}
	
	public void info(string s)
	{
		labelColor = Color.green;
		labelMsg = s;
	}
	
	public void login()
	{
		info("connect to server...");
		KBEngine.Event.fireIn("login", stringAccount, stringPasswd, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
	}
	
	public void createAccount()
	{
		info("connect to server...");
		
		KBEngine.Event.fireIn("createAccount", stringAccount, stringPasswd, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
	}
	
	public void onCreateAccountResult(UInt16 retcode, byte[] datas)
	{
		if(retcode != 0)
		{
			err("createAccount has error! err=" + KBEngineApp.app.serverErr(retcode));
			return;
		}
		
		if(KBEngineApp.validEmail(stringAccount))
		{
			info("Account created successfully, Please activate your Email!");
		}
		else
		{
			info("Account created successfully!");
		}
	}
	
	public void onConnectionState(bool success)
	{
		if(!success)
			err("connect(" + KBEngineApp.app.getInitArgs().ip + ":" + KBEngineApp.app.getInitArgs().port + ") error!");
		else
			info("connected successfully, please wait...");
	}
	
	public void onLoginFailed(UInt16 failedcode, byte[] serverdatas)
	{
        if (failedcode == 20)
		{
            err("login failed, err=" + KBEngineApp.app.serverErr(failedcode) + ", " + System.Text.Encoding.ASCII.GetString(serverdatas));
        }
		else
		{
			err("login failed, err=" + KBEngineApp.app.serverErr(failedcode));
		}
	}
	
	public void onVersionNotMatch(string verInfo, string serVerInfo)
	{
		err("");
	}

	public void onScriptVersionNotMatch(string verInfo, string serVerInfo)
	{
		err("");
	}
	
	public void onLoginBaseappFailed(UInt16 failedcode)
	{
		err("loginBaseapp failed, err=" + KBEngineApp.app.serverErr(failedcode));
	}
	
	public void onLoginBaseapp()
	{
		info("connect to loginBaseapp, please wait...");
	}

	public void onReloginBaseappFailed(UInt16 failedcode)
	{
		err("relogin failed, err=" + KBEngineApp.app.serverErr(failedcode));
		startRelogin = false;
	}
	
	public void onReloginBaseappSuccessfully()
	{
		info("relogin successfully!");
		startRelogin = false;
	}
	
	public void onLoginSuccessfully(UInt64 rndUUID, Int32 eid, Account accountEntity)
	{
		info("login successfully!");
		ui_state = 1;

		SceneManager.LoadScene("selavatars");
	}

	public void onKicked(UInt16 failedcode)
	{
		err("kick, disconnect!, reason=" + KBEngineApp.app.serverErr(failedcode));
		SceneManager.LoadScene("login");
		ui_state = 0;
	}

	public void Loginapp_importClientMessages()
	{
		info("Loginapp_importClientMessages ...");
	}

	public void Baseapp_importClientMessages()
	{
		info("Baseapp_importClientMessages ...");
	}
	
	public void Baseapp_importClientEntityDef()
	{
		info("importClientEntityDef ...");
	}
	
	public void onReqAvatarList(Dictionary<UInt64, AVATAR_INFOS> avatarList)
	{
		ui_avatarList = avatarList;
	}
	
	public void onCreateAvatarResult(UInt64 retcode, AVATAR_INFOS info, Dictionary<UInt64, AVATAR_INFOS> avatarList)
	{
		if(retcode != 0)
		{
			err("Error creating avatar, errcode=" + retcode);
			return;
		}
		
		onReqAvatarList(avatarList);
	}
	
	public void onRemoveAvatar(UInt64 dbid, Dictionary<UInt64, AVATAR_INFOS> avatarList)
	{
		if(dbid == 0)
		{
			err("Delete the avatar error!");
			return;
		}
		
		onReqAvatarList(avatarList);
	}
	
	public void onDisconnected()
	{
		err("disconnect! will try to reconnect...");
		startRelogin = true;
		Invoke("onReloginBaseappTimer", 1.0f);
	}
	
	public void onReloginBaseappTimer() 
	{
		if(ui_state == 0)
		{
			err("disconnect! ");
			return;
		}
	
		KBEngineApp.app.reloginBaseapp();
		
		if(startRelogin)
			Invoke("onReloginBaseappTimer", 3.0f);
	}
}

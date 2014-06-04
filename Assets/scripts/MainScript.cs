using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {
	
	private static Rect titleRect;
	private static Rect instructionsRect;
	private static Rect nextRect;
	private static GUIStyle titleStyle;
	private static GUIStyle instructionsStyle;
	private static GUIStyle nextStyle;
	
	private static Rect scoreRect;
	private static Rect highRect;
	private static GUIStyle scoreStyle = new GUIStyle();
	private static GUIStyle highScoreStyle = new GUIStyle();
	
	private static Rect gameOverRect;
	private static GUIStyle gameOverStyle = new GUIStyle();
	
	GameObject player;
	GameObject lastCreated;
	System.Random random = new System.Random();
	
	public static float height;
	public static float width;
	public static float playerSize;
	public static float openingWidth;
	public static float maxPlatformStep;
	
	//private static bool polledAcc;
	//public static Vector3 zeroAcc;
	
	public static bool running;
	public static bool fastForward;
	public static float platformSpeed;
	
	private float score;
	private float highScore;
	
	private bool displayGameOver;
	private bool displayGameStart;
	
	private const float GUI_RATIO_NORM = 600;
	public static float GUI_RATIO;
	
	private bool waitForTouchRelease;
	
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		waitForTouchRelease = false;
		
		highScore = PlayerPrefs.GetFloat("highScore", highScore);
		
		int nativeWidth = Screen.width;
		int nativeHeight = Screen.height;
		
		GUI_RATIO = nativeWidth / GUI_RATIO_NORM;
		
		Camera camera = Camera.main;
		//7.5f is the magic width we need to have
		float newOrthoSize = ((7.5f * nativeHeight) / (nativeWidth))/ 2;
		camera.orthographicSize = newOrthoSize;
		Vector3 newCameraLocation = camera.transform.position;
		newCameraLocation.y = newOrthoSize;
		camera.transform.position = newCameraLocation;
		
		height = 2f * camera.orthographicSize;
		width = height * camera.aspect;
		
		player = GameObject.Find("Player");
		playerSize = player.renderer.bounds.size.x;
		openingWidth = playerSize * 2;
		
		maxPlatformStep = playerSize / 6;
		fastForward = false;
		
		scoreRect = new Rect(10, 10, nativeWidth - 20, 100);
		highRect = new Rect(10, 10, nativeWidth - 20, 100);
		
		scoreStyle = new GUIStyle();
		scoreStyle.fontSize = (int) (48 * GUI_RATIO);
		scoreStyle.normal.textColor = Color.red;
		scoreStyle.alignment = TextAnchor.UpperLeft;
		
		highScoreStyle = new GUIStyle();
		highScoreStyle.fontSize = (int) (48 * GUI_RATIO);
		highScoreStyle.normal.textColor = Color.black;
		highScoreStyle.alignment = TextAnchor.UpperRight;
		
		gameOverRect = new Rect(10, 10, nativeWidth - 20, nativeHeight - 10);
		
		gameOverStyle = new GUIStyle();
		gameOverStyle.fontSize = (int) (56 * GUI_RATIO);
		gameOverStyle.normal.textColor = Color.red;
		gameOverStyle.alignment = TextAnchor.MiddleCenter;
		gameOverStyle.wordWrap = true;
		
		titleRect = new Rect(0, 0, nativeWidth - 20, nativeHeight / 3);
		instructionsRect = new Rect(0, nativeHeight / 3, nativeWidth - 20, nativeHeight / 3);
		nextRect = new Rect(0, (nativeHeight * 2) / 3, nativeWidth - 20, nativeHeight / 3);
		
		titleStyle = new GUIStyle();
		titleStyle.fontSize = (int) (48 * GUI_RATIO);
		titleStyle.normal.textColor = Color.black;
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.wordWrap = true;
		
		instructionsStyle = new GUIStyle();
		instructionsStyle.fontSize = (int) (28 * GUI_RATIO);
		instructionsStyle.normal.textColor = Color.black;
		instructionsStyle.alignment = TextAnchor.MiddleCenter;
		instructionsStyle.wordWrap = true;
		
		nextStyle = new GUIStyle();
		nextStyle.fontSize = (int) (24 * GUI_RATIO);
		nextStyle.normal.textColor = Color.black;
		nextStyle.alignment = TextAnchor.MiddleCenter;
		nextStyle.wordWrap = true;
		
		//polledAcc = false;
		
		running = true;
		displayGameOver = false;		
		displayGameStart = true;
		
		lastCreated = null;
		
		platformSpeed = 1.0f;
	}
	
	void startScene() {
		player.transform.position = new Vector3(width/2, height * 0.75f, 0);
		score = 0f;
		platformSpeed = 1.0f;
		lastCreated = null;
		foreach(PlatformScript platform in FindObjectsOfType(typeof(PlatformScript)) as PlatformScript[]) {
			platform.DestroyOnNextUpdate();
		}
		running = true;
		displayGameOver = false;
		displayGameStart = false;
	}
	
	void OnGUI() {
		if (!displayGameStart) {
			DrawOutline(scoreRect, ((long) (score * 10)).ToString(), scoreStyle, scoreStyle.normal.textColor, InvertColor(scoreStyle.normal.textColor));
			DrawOutline(highRect, ((long) (highScore * 10)).ToString(), highScoreStyle, highScoreStyle.normal.textColor, InvertColor(highScoreStyle.normal.textColor));
		}
		
		if (displayGameStart) {
			DrawOutline(titleRect, "Go Go Gravity", titleStyle, titleStyle.normal.textColor, InvertColor(titleStyle.normal.textColor));
			DrawOutline(instructionsRect, "Tilt left and right to roll\n\nPress and hold to fast-forward", instructionsStyle, instructionsStyle.normal.textColor, InvertColor(instructionsStyle.normal.textColor));
			DrawOutline(nextRect, "Press anywhere to start!", nextStyle, nextStyle.normal.textColor, InvertColor(nextStyle.normal.textColor));
		}
		
		if (displayGameOver) {
			DrawOutline(gameOverRect, "Game Over\n\nPress anywhere to continue", gameOverStyle, gameOverStyle.normal.textColor, InvertColor(gameOverStyle.normal.textColor));
		}
	}
	
	void Update () {		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
		
		bool touch = Input.touchCount > 0 || Input.GetMouseButtonDown(0);
		
		if (waitForTouchRelease && !touch) {
			waitForTouchRelease = false;
		}
		
		if (!displayGameStart && running) {
			if (player.transform.position.y >= height) {
				running = false;
				displayGameOver = true;
			    highScore = System.Math.Max(highScore, score);
				PlayerPrefs.SetFloat("highScore", highScore);
				
				waitForTouchRelease = touch;
			}
		}
		
		if (running) {
			//if (!polledAcc && Time.time > 1) {
			//	zeroAcc = Input.acceleration;
			//	polledAcc = true;
			//}
			
			//if (Input.GetKey(KeyCode.DownArrow) || (polledAcc && Input.acceleration.y - zeroAcc.y < -0.33)) {
			if (Input.GetKey(KeyCode.DownArrow) || Input.touchCount > 0 || Input.GetMouseButton(0)) {
				fastForward = true;
				if (!displayGameStart) score += maxPlatformStep;
			} else {
				fastForward = false;
				if (!displayGameStart) score += System.Math.Min(platformSpeed * Time.deltaTime, maxPlatformStep);
			}
			
			if (lastCreated == null || lastCreated.transform.position.y >= (height / 4) - playerSize) {
				float opening = (width - openingWidth) * (float) random.NextDouble();
				
				GameObject left = createPlatform();
		        left.transform.position = new Vector3(opening - (width / 2), -playerSize, 0);
				
				GameObject right = createPlatform();
		        right.transform.position = new Vector3(opening + openingWidth + (width / 2), -playerSize, 0);
				
				lastCreated = left;
				
				if (!displayGameStart) {
					platformSpeed *= 1.05f;
					score += 1;
				}
			}
		}
		
		if (displayGameOver || displayGameStart) {
			if (touch && !waitForTouchRelease) {
				startScene();
			}
		}
	}
	
	private GameObject createPlatform() {
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.transform.localScale = new Vector3 (width, 0.1f * GUI_RATIO, 1);
		platform.renderer.material.color = Color.white;
		Rigidbody rigidbody = platform.AddComponent<Rigidbody>();
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		platform.AddComponent("PlatformScript");		
		return platform;
	}
	
	public static void DrawOutline(Rect position, string text, GUIStyle style, Color inColor, Color outColor){
		int offset = 2;
		
	    var backupStyle = style;
	    style.normal.textColor = outColor;
	    position.x -= offset;
	    GUI.Label(position, text, style);
	    position.x += offset * 2;
	    GUI.Label(position, text, style);
	    position.x -= offset;
	    position.y -= offset;
	    GUI.Label(position, text, style);
	    position.y += offset * 2;
	    GUI.Label(position, text, style);
	    position.y -= offset;
	    style.normal.textColor = inColor;
	    GUI.Label(position, text, style);
	    style = backupStyle;
	}
	
	public static Color InvertColor (Color color) {
    	return new Color (1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
	}
	
}

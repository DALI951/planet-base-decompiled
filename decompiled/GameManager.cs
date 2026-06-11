using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x0200011F RID: 287
	public class GameManager
	{
		// Token: 0x06000890 RID: 2192 RVA: 0x000328B8 File Offset: 0x00030AB8
		public GameManager()
		{
			foreach (GameObject gameObject in Object.FindObjectsOfType<GameObject>())
			{
				if (!gameObject.CompareTag("RootObject") && gameObject.transform.parent == null)
				{
					Object.Destroy(gameObject);
				}
			}
			CultureInfo cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			cultureInfo.NumberFormat = CultureInfo.InvariantCulture.NumberFormat;
			CultureInfo.CurrentCulture = cultureInfo;
			ErrorManager.init();
			Debug.Log("Game Starting, version: " + Definitions.VersionNumber.ToString());
			Debug.Log(string.Concat(new string[]
			{
				"Time: ",
				DateTime.Now.ToString(),
				", UnityDebug: ",
				Debug.isDebugBuild.ToString(),
				", Debug: ",
				false.ToString(),
				", Profiling: ",
				false.ToString()
			}));
			if (!Application.unityVersion.StartsWith("2022.3.21"))
			{
				throw new Exception("You need to run the game in Unity 2022.3.21");
			}
			SoundList.getInstance().check();
			Profile instance = Singleton<Profile>.getInstance();
			instance.load();
			QualitySettings.SetQualityLevel(instance.getQualitySetting());
			GameManager.initQualitySettings();
			this.setGameStateLogo();
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x00032A27 File Offset: 0x00030C27
		public static void initQualitySettings()
		{
			CameraManager.getInstance().setAntiAliasingEnabled(Singleton<Profile>.getInstance().getAaSetting() == 1);
			Shader.globalMaximumLOD = ExtraQualitySettings.getQualityLevelSettings().ShaderMaximumLod;
			QualitySettings.vSyncCount = Singleton<Profile>.getInstance().getVsyncSetting();
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x00032A60 File Offset: 0x00030C60
		public void onGui()
		{
			if (Event.current != null && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.SysReq && !Event.current.shift)
			{
				this.takeScreenshot();
			}
			this.mStopwatch.Start();
			if (this.mGameState != null && this.mState == GameManager.State.Updating)
			{
				this.mGameState.onGui();
			}
			if (this.mGameState != null && this.mGameState.shouldFadeIn() && this.mFadeInTimer > 0f)
			{
				this.mScreenRenderer.renderFade(this.mFadeInTimer);
			}
			if (this.mGameState != null && this.mGameState.shouldShowLoadingScreen() && (this.mState == GameManager.State.Loading || this.mState == GameManager.State.Waiting))
			{
				if (Singleton<ChallengeManager>.getInstance().isChallengeEnabled())
				{
					this.mScreenRenderer.renderLoadingChallenge(Singleton<ChallengeManager>.getInstance().getCurrentChallenge(), this.mState == GameManager.State.Waiting);
				}
				else
				{
					this.mScreenRenderer.renderLoading(PlanetManager.getCurrentPlanet().getDefinition().LoadingBackground, this.mState == GameManager.State.Waiting);
				}
			}
			if (this.mGameState.isTitleState())
			{
				Singleton<TitleScene>.getInstance().onGui(this.mGameState.shouldDrawAnnouncement());
			}
			ErrorManager.render();
			Singleton<GuiStyles>.getInstance().update();
			this.mStopwatch.Stop();
			DebugRenderer.clearBuildInfo();
			if (!Input.GetKey(KeyCode.Space) && Debug.isDebugBuild)
			{
				DebugRenderer.addBuildInfo("UnityDebug");
			}
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x00032BD0 File Offset: 0x00030DD0
		public void fixedUpdate(float timeStep)
		{
			if (this.mPendingGameState != null)
			{
				this.onNewGameState(this.mPendingGameState);
			}
			GameManager.mInstance = this;
			Singleton<AudioPlayer>.getInstance().update(timeStep);
			this.mStopwatch.Start();
			if (this.mGameState != null)
			{
				if (this.mState == GameManager.State.Loading)
				{
					this.mUpdateTimeMicros = 0L;
					this.mLoadingWaitTimer -= timeStep;
					if (this.mLoadingWaitTimer < 0f && this.mGameState.load())
					{
						if (this.mGameState.shouldShowLoadingScreen())
						{
							this.mState = GameManager.State.Waiting;
						}
						else
						{
							this.mState = GameManager.State.Updating;
						}
					}
				}
				else if (this.mState == GameManager.State.Waiting)
				{
					if (Input.anyKey || Input.GetMouseButtonDown(0))
					{
						this.mGameState.onStartUpdating();
						this.mState = GameManager.State.Updating;
					}
				}
				else if (this.mState == GameManager.State.Updating)
				{
					this.mGameState.fixedUpdate(timeStep, this.mFrameIndex);
				}
			}
			CameraManager.getInstance().fixedUpdate(Time.smoothDeltaTime, this.mFrameIndex);
			Singleton<ParticleManager>.getInstance().update(timeStep);
			this.mStopwatch.Stop();
			long num = 1000000L * this.mStopwatch.ElapsedTicks / Stopwatch.Frequency;
			this.mUpdateTimeMicros = (this.mUpdateTimeMicros * 99L + num) / 100L;
			this.mStopwatch.Reset();
			this.mFrameIndex = (this.mFrameIndex + 1) & 63;
			Singleton<ServerInterfaceManager>.getInstance().update(timeStep);
			ErrorManager.update(timeStep);
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x00032D40 File Offset: 0x00030F40
		public void update(float timeStep)
		{
			if (this.mState == GameManager.State.Updating)
			{
				this.mGameState.update(timeStep);
				CameraManager.getInstance().update(Time.smoothDeltaTime);
			}
			else
			{
				this.mScreenRenderer.update(timeStep);
			}
			if (this.mGameState.isTitleState())
			{
				Singleton<TitleScene>.getInstance().update(timeStep);
			}
			if (this.mState == GameManager.State.Updating && this.mFadeInTimer > 0f)
			{
				this.mFadeInTimer -= timeStep;
				if (this.mFadeInTimer < 0f)
				{
					this.mFadeInTimer = 0f;
				}
			}
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x00032DD4 File Offset: 0x00030FD4
		private string getScreenshotPath()
		{
			string text = Util.getFilesFolder() + "/Screenshots";
			string text2 = text + "/screenshot" + DateTime.Now.ToString("_h_mm_ss_ff") + ".png";
			Debug.Log("Saved screenshot to: " + text2);
			Directory.CreateDirectory(text);
			return text2;
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00032E2C File Offset: 0x0003102C
		private void takeScreenshot()
		{
			string screenshotPath = this.getScreenshotPath();
			ScreenCapture.CaptureScreenshot(screenshotPath);
			ErrorManager.setToastMessage(StringList.get("screenshot_taken") + ": " + Path.GetFileName(screenshotPath), 3f);
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x00032E6C File Offset: 0x0003106C
		private void takeHiResScreenshot()
		{
			string screenshotPath = this.getScreenshotPath();
			ScreenCapture.CaptureScreenshot(screenshotPath, 2);
			ErrorManager.setToastMessage(StringList.get("screenshot_taken") + ": " + Path.GetFileName(screenshotPath), 3f);
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x00032EAB File Offset: 0x000310AB
		public bool isEndOfFramePending()
		{
			return this.mGameState.isEndOfFramePending();
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00032EB8 File Offset: 0x000310B8
		public void onEndOfFrame()
		{
			this.mGameState.onEndOfFrame();
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x00032EC5 File Offset: 0x000310C5
		public void onQuit()
		{
			Singleton<ServerInterfaceManager>.getInstance().destroy();
			Debug.Log("Application quitting");
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x00032EDB File Offset: 0x000310DB
		public void OnDestroy()
		{
			if (this.mGameState != null)
			{
				this.mGameState.destroy();
				this.mGameState = null;
			}
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x00032EF7 File Offset: 0x000310F7
		public void setGameStateLogo()
		{
			this.setNewState(new GameStateLogo());
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00032F04 File Offset: 0x00031104
		public void setGameStateTitle()
		{
			this.setNewState(new GameStateTitle(this.mGameState));
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00032F17 File Offset: 0x00031117
		public void setGameStateLoadGame()
		{
			this.setNewState(new GameStateLoadGame());
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x00032F24 File Offset: 0x00031124
		public void setGameStateControls()
		{
			this.setNewState(new GameStateControls());
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x00032F31 File Offset: 0x00031131
		public void setGameStateLocationSelection()
		{
			this.setNewState(new GameStateLocationSelection());
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x00032F3E File Offset: 0x0003113E
		public void setGameStateChallengeSelection()
		{
			this.setNewState(new GameStateChallengeSelection());
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00032F4B File Offset: 0x0003114B
		public void setGameStateGameNew(int seed, int planetIndex, bool tutorial, Challenge challenge)
		{
			this.setNewState(new GameStateGame(seed, planetIndex, tutorial, challenge));
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00032F60 File Offset: 0x00031160
		public void setGameStateGameContinue()
		{
			SaveData mostRecentSave = SaveData.getMostRecentSave();
			this.setNewState(new GameStateGame(mostRecentSave.getPath(), mostRecentSave.getPlanetIndex(), mostRecentSave.createChallenge()));
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00032F90 File Offset: 0x00031190
		public void setGameStateGameLoad(string name, int planetIndex, Challenge challenge)
		{
			this.setNewState(new GameStateGame(name, planetIndex, challenge));
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x00032FA0 File Offset: 0x000311A0
		public void setGameStateSettings()
		{
			this.setNewState(new GameStateSettings());
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00032FAD File Offset: 0x000311AD
		public void setGameStateCredits()
		{
			this.setNewState(new GameStateCredits());
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00032FBA File Offset: 0x000311BA
		public void setGameStateAchievements()
		{
			this.setNewState(new GameStateAchievements());
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00032FC7 File Offset: 0x000311C7
		public void setGameStateWorkshopUploader()
		{
			this.setNewState(new GameStateWorkshopUploader());
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x00032FD4 File Offset: 0x000311D4
		public void setNewState(GameState gameState)
		{
			this.mLoadingWaitTimer = gameState.getLoadingWaitTime();
			this.mPendingGameState = gameState;
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00032FEC File Offset: 0x000311EC
		private void onNewGameState(GameState gameState)
		{
			if (!gameState.isTitleState() && this.mGameState != null && this.mGameState.isTitleState())
			{
				Singleton<TitleScene>.getInstance().destroy();
			}
			if (gameState.shouldShowLoadingScreen())
			{
				this.mScreenRenderer.loadHint();
				Singleton<MusicManager>.getInstance().onLoading();
			}
			this.mFadeInTimer = 1f;
			Debug.Log("New game state: " + gameState.GetType().Name);
			if (this.mGameState != null)
			{
				this.mGameState.destroy();
			}
			if (gameState.isTitleState() && (this.mGameState == null || !this.mGameState.isTitleState()))
			{
				Singleton<TitleScene>.getInstance().create();
			}
			this.mGameState = gameState;
			this.mPendingGameState = null;
			this.mState = GameManager.State.Loading;
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x000330B1 File Offset: 0x000312B1
		public GameState getGameState()
		{
			return this.mGameState;
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x000330B9 File Offset: 0x000312B9
		public static GameManager getInstance()
		{
			if (GameManager.mInstance == null)
			{
				GameManager.mInstance = new GameManager();
			}
			return GameManager.mInstance;
		}

		// Token: 0x04000690 RID: 1680
		private static GameManager mInstance;

		// Token: 0x04000691 RID: 1681
		private ScreenRenderer mScreenRenderer = new ScreenRenderer();

		// Token: 0x04000692 RID: 1682
		private GameState mGameState;

		// Token: 0x04000693 RID: 1683
		private GameState mPendingGameState;

		// Token: 0x04000694 RID: 1684
		private GameManager.State mState;

		// Token: 0x04000695 RID: 1685
		private Stopwatch mStopwatch = new Stopwatch();

		// Token: 0x04000696 RID: 1686
		private long mUpdateTimeMicros;

		// Token: 0x04000697 RID: 1687
		private int mFrameIndex;

		// Token: 0x04000698 RID: 1688
		private float mFadeInTimer = 1f;

		// Token: 0x04000699 RID: 1689
		private float mLoadingWaitTimer;

		// Token: 0x020002BC RID: 700
		public enum State
		{
			// Token: 0x04000E44 RID: 3652
			Loading,
			// Token: 0x04000E45 RID: 3653
			Waiting,
			// Token: 0x04000E46 RID: 3654
			Updating
		}
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modio.Unity.UI.Input
{
	public class ModioUIPromptIconResolver : MonoBehaviour
	{
		[return: TupleElementNames(new string[]
		{
			"icon",
			"displayAsText"
		})]
		public ValueTuple<Sprite, string> TryGetKeyboardIcon(string controlPath)
		{
			foreach (ModioUIPromptIconResolver.KeyboardMapping keyboardMapping in this._keyboardMappings)
			{
				if (keyboardMapping.controlPath == controlPath)
				{
					return new ValueTuple<Sprite, string>(keyboardMapping.icon, keyboardMapping.displayAsText);
				}
			}
			return default(ValueTuple<Sprite, string>);
		}

		public Sprite ResolveIcon(string controlPath, RuntimePlatform forControllerType)
		{
			foreach (ModioUIPromptIconResolver.PlatformSprites platformSprites in this._platforms)
			{
				if (platformSprites.forControllerTypes.Contains(forControllerType))
				{
					return platformSprites.GetSprite(controlPath);
				}
			}
			return null;
		}

		[SerializeField]
		private ModioUIPromptIconResolver.PlatformSprites[] _platforms;

		[SerializeField]
		private ModioUIPromptIconResolver.KeyboardMapping[] _keyboardMappings;

		[Serializable]
		private class KeyboardMapping
		{
			public string controlPath;

			public Sprite icon;

			public string displayAsText;
		}

		[Serializable]
		private class PlatformSprites
		{
			public Sprite GetSprite(string controlPath)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(controlPath);
				if (num <= 2126255620U)
				{
					if (num <= 996955572U)
					{
						if (num <= 527024624U)
						{
							if (num != 297952813U)
							{
								if (num == 527024624U)
								{
									if (controlPath == "leftStick")
									{
										return this.leftStick;
									}
								}
							}
							else if (controlPath == "select")
							{
								return this.selectButton;
							}
						}
						else if (num != 881114153U)
						{
							if (num == 996955572U)
							{
								if (controlPath == "dpad/up")
								{
									return this.dpadUp;
								}
							}
						}
						else if (controlPath == "dpad/right")
						{
							return this.dpadRight;
						}
					}
					else if (num <= 1697318111U)
					{
						if (num != 1411432780U)
						{
							if (num == 1697318111U)
							{
								if (controlPath == "start")
								{
									return this.startButton;
								}
							}
						}
						else if (controlPath == "leftTrigger")
						{
							return this.leftTrigger;
						}
					}
					else if (num != 2106031334U)
					{
						if (num != 2113007854U)
						{
							if (num == 2126255620U)
							{
								if (controlPath == "dpad")
								{
									return this.dpad;
								}
							}
						}
						else if (controlPath == "rightStickPress")
						{
							return this.rightStickPress;
						}
					}
					else if (controlPath == "buttonSouth")
					{
						return this.buttonSouth;
					}
				}
				else if (num <= 2991474140U)
				{
					if (num <= 2427478531U)
					{
						if (num != 2139186104U)
						{
							if (num == 2427478531U)
							{
								if (controlPath == "rightTrigger")
								{
									return this.rightTrigger;
								}
							}
						}
						else if (controlPath == "buttonEast")
						{
							return this.buttonEast;
						}
					}
					else if (num != 2803379527U)
					{
						if (num != 2866927184U)
						{
							if (num == 2991474140U)
							{
								if (controlPath == "dpad/left")
								{
									return this.dpadLeft;
								}
							}
						}
						else if (controlPath == "buttonNorth")
						{
							return this.buttonNorth;
						}
					}
					else if (controlPath == "rightStick")
					{
						return this.rightStick;
					}
				}
				else if (num <= 3792611837U)
				{
					if (num != 3616964664U)
					{
						if (num == 3792611837U)
						{
							if (controlPath == "rightShoulder")
							{
								return this.rightShoulder;
							}
						}
					}
					else if (controlPath == "leftShoulder")
					{
						return this.leftShoulder;
					}
				}
				else if (num != 4104197953U)
				{
					if (num != 4167788306U)
					{
						if (num == 4285979527U)
						{
							if (controlPath == "leftStickPress")
							{
								return this.leftStickPress;
							}
						}
					}
					else if (controlPath == "buttonWest")
					{
						return this.buttonWest;
					}
				}
				else if (controlPath == "dpad/down")
				{
					return this.dpadDown;
				}
				return null;
			}

			[SerializeField]
			public List<RuntimePlatform> forControllerTypes;

			public Sprite buttonSouth;

			public Sprite buttonNorth;

			public Sprite buttonEast;

			public Sprite buttonWest;

			public Sprite startButton;

			public Sprite selectButton;

			public Sprite leftTrigger;

			public Sprite rightTrigger;

			public Sprite leftShoulder;

			public Sprite rightShoulder;

			public Sprite dpad;

			public Sprite dpadUp;

			public Sprite dpadDown;

			public Sprite dpadLeft;

			public Sprite dpadRight;

			public Sprite leftStick;

			public Sprite rightStick;

			public Sprite leftStickPress;

			public Sprite rightStickPress;
		}
	}
}

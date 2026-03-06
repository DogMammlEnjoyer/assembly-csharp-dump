using System;

namespace Modio.Errors
{
	public static class ErrorExtensions
	{
		public static string GetMessage(this ApiErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this ArchiveErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this ErrorCode errorCode, string append = null)
		{
			string text;
			if (errorCode <= ErrorCode.USER_NO_MOD_RATING)
			{
				if (errorCode <= ErrorCode.OPEN_IDNOT_CONFIGURED)
				{
					if (errorCode <= ErrorCode.APIKEY_FOR_TEST_ONLY)
					{
						long num = errorCode - ErrorCode.HTTP_NOT_INITIALIZED;
						if (num <= 80L)
						{
							switch ((uint)num)
							{
							case 0U:
								text = "HTTP service not initialized";
								goto IL_A9C;
							case 1U:
								text = "HTTP service already initialized";
								goto IL_A9C;
							case 2U:
								text = "Unable to connect to server";
								goto IL_A9C;
							case 3U:
								text = "Insufficient permissions";
								goto IL_A9C;
							case 4U:
								text = "Invalid platform HTTP security configuration";
								goto IL_A9C;
							case 5U:
								text = "Unable to connect to server";
								goto IL_A9C;
							case 6U:
								text = "Invalid endpoint path";
								goto IL_A9C;
							case 7U:
								text = "Exceeded the allowed number of redirects";
								goto IL_A9C;
							case 8U:
								text = "Server closed connection unexpectedly";
								goto IL_A9C;
							case 9U:
								text = "Trying to download file from outside of mod.io domain";
								goto IL_A9C;
							case 10U:
								text = "The mod.io servers are overloaded. Please wait a bit before trying again";
								goto IL_A9C;
							case 11U:
								text = "An error occurred making a HTTP request";
								goto IL_A9C;
							case 12U:
								text = "The HTTP response was malformed or not in the expected format";
								goto IL_A9C;
							case 13U:
								text = "Too many requests made to the mod.io API within the rate-limiting window. Please wait and try again";
								goto IL_A9C;
							case 14U:
								text = "Could not create folder";
								goto IL_A9C;
							case 15U:
								text = "Could not create file";
								goto IL_A9C;
							case 16U:
								text = "Insufficient permission for filesystem operation";
								goto IL_A9C;
							case 17U:
								text = "File locked (already in use?)";
								goto IL_A9C;
							case 18U:
								text = "File not found";
								goto IL_A9C;
							case 19U:
								text = "Directory not empty";
								goto IL_A9C;
							case 20U:
								text = "Error reading file";
								goto IL_A9C;
							case 21U:
								text = "Error writing file";
								goto IL_A9C;
							case 22U:
								text = "Directory not found";
								goto IL_A9C;
							case 23U:
								text = "Could not initialize user storage";
								goto IL_A9C;
							case 24U:
								text = "OAuth token was missing";
								goto IL_A9C;
							case 25U:
								text = "The user's OAuth token was invalid";
								goto IL_A9C;
							case 26U:
								text = "No Auth token available";
								goto IL_A9C;
							case 27U:
								text = "User is already authenticated. To use a new user and OAuth token, call ClearUserDataAsync";
								goto IL_A9C;
							case 28U:
								text = "Invalid user";
								goto IL_A9C;
							case 29U:
								text = "Some or all of the user data was missing from storage";
								goto IL_A9C;
							case 30U:
								text = "File did not have a valid archive header";
								goto IL_A9C;
							case 31U:
								text = "File uses an unsupported compression method. Please use STORE or DEFLATE";
								goto IL_A9C;
							case 32U:
								text = "The asynchronous operation was cancelled before it completed";
								goto IL_A9C;
							case 33U:
								text = "The asynchronous operation produced an error before it completed";
								goto IL_A9C;
							case 34U:
								text = "Operating system could not create the requested handle";
								goto IL_A9C;
							case 35U:
								text = "No data available";
								goto IL_A9C;
							case 36U:
								text = "End of file";
								goto IL_A9C;
							case 37U:
								text = "Operation could not be started as the service queue was missing or destroyed";
								goto IL_A9C;
							case 38U:
								text = "mod.io SDK was already initialized";
								goto IL_A9C;
							case 39U:
								text = "mod.io SDK was not initialized";
								goto IL_A9C;
							case 40U:
								text = "Index out of range";
								goto IL_A9C;
							case 41U:
								text = "Bad parameter supplied";
								goto IL_A9C;
							case 42U:
								text = "mod.io SDK is shutting down, operation is cancelled";
								goto IL_A9C;
							case 43U:
								text = "mod.io SDK could not find required components";
								goto IL_A9C;
							case 44U:
								text = "A low-level system error occured, refer to the logs for code and location";
								goto IL_A9C;
							case 45U:
								text = "Need more input data";
								goto IL_A9C;
							case 46U:
								text = "End of deflate stream";
								goto IL_A9C;
							case 47U:
								text = "Stream error";
								goto IL_A9C;
							case 48U:
								text = "Invalid block type";
								goto IL_A9C;
							case 49U:
								text = "Invalid store block length";
								goto IL_A9C;
							case 50U:
								text = "Too many symbols";
								goto IL_A9C;
							case 51U:
								text = "Invalid code lengths";
								goto IL_A9C;
							case 52U:
								text = "Invalid bit length repeat";
								goto IL_A9C;
							case 53U:
								text = "Missing end-of-block marker";
								goto IL_A9C;
							case 54U:
								text = "Invalid literal length";
								goto IL_A9C;
							case 55U:
								text = "Invalid distance code";
								goto IL_A9C;
							case 56U:
								text = "Invalid distance";
								goto IL_A9C;
							case 57U:
								text = "Over-subscribed length";
								goto IL_A9C;
							case 58U:
								text = "Incomplete length set";
								goto IL_A9C;
							case 59U:
								text = "Internal: No mods require processing for this iteration";
								goto IL_A9C;
							case 60U:
								text = "The current mod installation or update was cancelled";
								goto IL_A9C;
							case 61U:
								text = "Could not perform operation: Mod management is disabled and mod collection is locked";
								goto IL_A9C;
							case 62U:
								text = "Mod management was already enabled and the callback remains unchanged.";
								goto IL_A9C;
							case 63U:
								text = "The current modfile upload was cancelled";
								goto IL_A9C;
							case 64U:
								text = "The specified mod's files are currently being updated by the SDK. Please try again later.";
								goto IL_A9C;
							case 65U:
								text = "Temporary mod set was not initialized. Please call InitTempModSet.";
								goto IL_A9C;
							case 66U:
								text = "The dependencies for this mod are incompatible with your version of the game. Please contact the mod creator for support.";
								goto IL_A9C;
							case 67U:
								text = "Mod directory does not contain any files";
								goto IL_A9C;
							case 68U:
								text = "Mod directory does not exist";
								goto IL_A9C;
							case 69U:
								text = "Mod MD5 does not match";
								goto IL_A9C;
							case 70U:
								text = "The display price for the mod was out-of-date or incorrect. Please retry with the correct display price.";
								goto IL_A9C;
							case 71U:
								text = "A failure has occured when trying to authenticate with the monetization system.";
								goto IL_A9C;
							case 72U:
								text = "Unable to fetch the account's wallet. Please confirm the account has one";
								goto IL_A9C;
							case 73U:
								text = "The game does not have active monetization.";
								goto IL_A9C;
							case 74U:
								text = "The payment transaction failed. Please try again later.";
								goto IL_A9C;
							case 75U:
								text = "The given display price does not match the price of the mod.";
								goto IL_A9C;
							case 76U:
								text = "The account already owns this item.";
								goto IL_A9C;
							case 77U:
								text = "The account has insufficent funds to make this purchase.";
								goto IL_A9C;
							case 78U:
								text = "Some entitlements could not be verified. Please try again.";
								goto IL_A9C;
							case 79U:
								text = "The configured Metrics Secret Key is invalid.";
								goto IL_A9C;
							case 80U:
								text = "A mod installation has previously failed, can't install all needed mods for this temporary mod session.";
								goto IL_A9C;
							}
						}
						long num2 = errorCode - ErrorCode.MODIO_OUTAGE;
						if (num2 <= 3L)
						{
							switch ((uint)num2)
							{
							case 0U:
								text = "mod.io is currently experiencing an outage. (rare)";
								goto IL_A9C;
							case 1U:
								text = "Cross-origin request forbidden.";
								goto IL_A9C;
							case 2U:
								text = "mod.io failed to complete the request, please try again. (rare)";
								goto IL_A9C;
							case 3U:
								text = "API version supplied is invalid.";
								goto IL_A9C;
							}
						}
						long num3 = errorCode - ErrorCode.MISSING_APIKEY;
						if (num3 <= 17L)
						{
							switch ((uint)num3)
							{
							case 0U:
								text = "api_key is missing from your request.";
								goto IL_A9C;
							case 1U:
								text = "api_key supplied is malformed.";
								goto IL_A9C;
							case 2U:
								text = "api_key supplied is invalid.";
								goto IL_A9C;
							case 3U:
								text = "Access token is missing the write scope to perform the request.";
								goto IL_A9C;
							case 4U:
								text = "Access token is missing the read scope to perform the request.";
								goto IL_A9C;
							case 5U:
								text = "Access token is expired, or has been revoked.";
								goto IL_A9C;
							case 6U:
								text = "Authenticated user account has been deleted.";
								goto IL_A9C;
							case 7U:
								text = "Authenticated user account has been banned by mod.io admins.";
								goto IL_A9C;
							case 8U:
								text = "You have been ratelimited for making too many requests. See Rate Limiting.";
								goto IL_A9C;
							case 9U:
								text = "You have been ratelimited from calling this endpoint again, for making too many requests. See Rate Limiting.";
								goto IL_A9C;
							case 12U:
								text = "Email login code has expired, please request a new one.";
								goto IL_A9C;
							case 14U:
								text = "Email login code is invalid";
								goto IL_A9C;
							case 16U:
								text = "The api_key supplied in the request must be associated with a game.";
								goto IL_A9C;
							case 17U:
								text = "The api_key supplied in the request is for test environment purposes only and cannot be used for this functionality.";
								goto IL_A9C;
							}
						}
					}
					else
					{
						if (errorCode == ErrorCode.CANNOT_VERIFY_EXTERNAL_CREDENTIALS)
						{
							text = "mod.io was unable to verify the credentials against the external service provider.";
							goto IL_A9C;
						}
						if (errorCode == ErrorCode.USER_NO_ACCEPT_TERMS_OF_USE)
						{
							text = "The user has not agreed to the mod.io Terms of Use. Please see terms_agreed parameter description and the Terms endpoint for more information.";
							goto IL_A9C;
						}
						if (errorCode == ErrorCode.OPEN_IDNOT_CONFIGURED)
						{
							text = "You must configure your OpenID config for your game in your game authentication settings before being able to authenticate users.";
							goto IL_A9C;
						}
					}
				}
				else if (errorCode <= ErrorCode.REQUESTED_GAME_NOT_FOUND)
				{
					long num4 = errorCode - ErrorCode.BINARY_FILE_CORRUPTED;
					if (num4 <= 8L)
					{
						switch ((uint)num4)
						{
						case 0U:
							text = "The submitted binary file is corrupted.";
							goto IL_A9C;
						case 1U:
							text = "The submitted binary file is unreadable.";
							goto IL_A9C;
						case 2U:
						case 7U:
							goto IL_A9A;
						case 3U:
							text = "You have used the input_json parameter with semantically incorrect JSON.";
							goto IL_A9C;
						case 4U:
							text = "The Content-Type header is missing from your request.";
							goto IL_A9C;
						case 5U:
							text = "The Content-Type header is not supported for this endpoint.";
							goto IL_A9C;
						case 6U:
							text = "You have requested a response format that is not supported (JSON only).";
							goto IL_A9C;
						case 8U:
							text = "The request contains validation errors for the data supplied. See the attached errors field within the Error Object to determine which input failed.";
							goto IL_A9C;
						}
					}
					if (errorCode == ErrorCode.REQUESTED_RESOURCE_NOT_FOUND)
					{
						text = "The requested resource does not exist.";
						goto IL_A9C;
					}
					if (errorCode == ErrorCode.REQUESTED_GAME_NOT_FOUND)
					{
						text = "The requested game could not be found.";
						goto IL_A9C;
					}
				}
				else if (errorCode <= ErrorCode.FORBIDDEN_TACNOT_ACCEPTED)
				{
					if (errorCode == ErrorCode.REQUESTED_GAME_DELETED)
					{
						text = "The requested game has been deleted.";
						goto IL_A9C;
					}
					long num5 = errorCode - ErrorCode.FORBIDDEN_DMCA;
					if (num5 <= 11L)
					{
						switch ((uint)num5)
						{
						case 0U:
							text = "This mod is currently under DMCA and the user cannot be subscribed to it.";
							goto IL_A9C;
						case 1U:
							text = "This mod is hidden and the user cannot be subscribed to it.";
							goto IL_A9C;
						case 4U:
							text = "The user is already subscribed to the specified mod";
							goto IL_A9C;
						case 5U:
							text = "The authenticated user is not subscribed to the mod.";
							goto IL_A9C;
						case 6U:
							text = "The authenticated user does not have permission to upload modfiles for the specified mod. Ensure the user is a team manager or administrator.";
							goto IL_A9C;
						case 10U:
							text = "The requested modfile could not be found.";
							goto IL_A9C;
						case 11U:
							text = "The item has not been accepted and can not be purchased at this time.";
							goto IL_A9C;
						}
					}
				}
				else
				{
					long num6 = errorCode - ErrorCode.INSUFFICIENT_PERMISSION;
					if (num6 <= 17L)
					{
						switch ((uint)num6)
						{
						case 0U:
							text = "The authenticated user does not have permission to delete this mod. This action is restricted to team managers and administrators only.";
							goto IL_A9C;
						case 1U:
							text = "This mod is missing a file and cannot be subscribed to.";
							goto IL_A9C;
						case 2U:
						case 5U:
						case 6U:
						case 8U:
						case 12U:
						case 13U:
						case 14U:
						case 15U:
							goto IL_A9A;
						case 3U:
							text = "The requested mod could not be found.";
							goto IL_A9C;
						case 4U:
							text = "The requested mod has been deleted.";
							goto IL_A9C;
						case 7U:
							text = "The requested comment could not be found.";
							goto IL_A9C;
						case 9U:
							text = "The authenticated user has already submitted a rating for this mod.";
							goto IL_A9C;
						case 10U:
							text = "The authenticated user does not have permission to submit reports on mod.io due to their access being revoked.";
							goto IL_A9C;
						case 11U:
							text = "The specified resource is not able to be reported at this time, this is potentially due to the resource in question being removed.";
							goto IL_A9C;
						case 16U:
							text = "The authenticated user does not have permission to modify this resource.";
							goto IL_A9C;
						case 17U:
							text = "The authenticated user does not have permission to modify this resource.";
							goto IL_A9C;
						}
					}
					if (errorCode == ErrorCode.USER_NO_MOD_RATING)
					{
						text = "The authenticated user cannot clear the mod rating as none exists.";
						goto IL_A9C;
					}
				}
			}
			else if (errorCode <= ErrorCode.MONETIZATION_WALLET_FETCH_FAILED)
			{
				if (errorCode <= ErrorCode.CANNOT_MUTE_YOURSELF)
				{
					if (errorCode == ErrorCode.MATURE_MODS_NOT_ALLOWED)
					{
						text = "This game does not allow mature mods.";
						goto IL_A9C;
					}
					if (errorCode == ErrorCode.MUTE_USER_NOT_FOUND)
					{
						text = "The user with the supplied UserID could not be found.";
						goto IL_A9C;
					}
					if (errorCode == ErrorCode.CANNOT_MUTE_YOURSELF)
					{
						text = "You cannot mute yourself.";
						goto IL_A9C;
					}
				}
				else
				{
					if (errorCode == ErrorCode.INSUFFICIENT_SPACE)
					{
						text = "Insufficient space for file";
						goto IL_A9C;
					}
					if (errorCode == ErrorCode.REQUESTED_USER_NOT_FOUND)
					{
						text = "The requested user could not be found.";
						goto IL_A9C;
					}
					long num7 = errorCode - ErrorCode.MONETIZATION_UNEXPECTED_ERROR;
					if (num7 <= 8L)
					{
						switch ((uint)num7)
						{
						case 0U:
							text = "An un expected error during a purchase transaction has occured. Please try again later.";
							goto IL_A9C;
						case 1U:
							text = "Unable to communicate with the monetization system. Please try again later.";
							goto IL_A9C;
						case 2U:
							text = "A failure has occured when trying to authenticate with the monetization system.";
							goto IL_A9C;
						case 7U:
							text = "The account has not been created with monetization.";
							goto IL_A9C;
						case 8U:
							text = "Unable to fetch the accounts' wallet. Please confirm the account has one";
							goto IL_A9C;
						}
					}
				}
			}
			else if (errorCode <= ErrorCode.MONETIZATION_GAME_MONETIZATION_NOT_ENABLED)
			{
				if (errorCode == ErrorCode.MONETIZATION_IN_MAINTENANCE)
				{
					text = "The monetization is currently in maintance mode. Please try again later.";
					goto IL_A9C;
				}
				if (errorCode == ErrorCode.USER_MONETIZATION_DISABLED)
				{
					text = "The account does not have monetization enabled.";
					goto IL_A9C;
				}
				if (errorCode == ErrorCode.MONETIZATION_GAME_MONETIZATION_NOT_ENABLED)
				{
					text = "The game does not have active monetization.";
					goto IL_A9C;
				}
			}
			else if (errorCode <= ErrorCode.MONETIZATION_ITEM_ALREADY_OWNED)
			{
				if (errorCode == ErrorCode.MONETIZATION_PAYMENT_FAILED)
				{
					text = "The payment transaction failed. Please try again later.";
					goto IL_A9C;
				}
				if (errorCode == ErrorCode.MONETIZATION_ITEM_ALREADY_OWNED)
				{
					text = "The account already owns this item.";
					goto IL_A9C;
				}
			}
			else
			{
				if (errorCode == ErrorCode.MONETIZATION_INCORRECT_DISPLAY_PRICE)
				{
					text = "The given display price does not match the price of the mod.";
					goto IL_A9C;
				}
				if (errorCode == ErrorCode.MONETIZATION_INSUFFICIENT_FUNDS)
				{
					text = "The account has insufficent funds to make this purchase.";
					goto IL_A9C;
				}
			}
			IL_A9A:
			text = null;
			IL_A9C:
			string text2 = text;
			return string.Format("{0}{1}{2}", errorCode, string.IsNullOrWhiteSpace(text2) ? string.Empty : (": " + text2), string.IsNullOrWhiteSpace(append) ? string.Empty : (": " + append));
		}

		public static string GetMessage(this FilesystemErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this GenericErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this HttpErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this MetricsErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this ModManagementErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this ModValidationErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this MonetizationErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this SystemErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this TempModsErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this UserAuthErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this UserDataErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}

		public static string GetMessage(this ZlibErrorCode errorCode, string append = null)
		{
			return ((ErrorCode)errorCode).GetMessage(append);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BizzyBeeGames;

namespace WordConnect
{
	[RequireComponent(typeof(Button))]
	public class RewardAdButton : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private int	coinsToReward;
		[SerializeField] private bool	testMode;

		#endregion

		#region Properties

		public Button Button { get { return gameObject.GetComponent<Button>(); } }

		#endregion

		#region Unity Methods

		private void Awake()
		{
			Button.onClick.AddListener(OnClick);

			gameObject.SetActive(false);

			MobileAdsManager.Instance.OnRewardAdLoaded	+= OnRewardAdLoaded;
			MobileAdsManager.Instance.OnAdsRemoved		+= OnAdsRemoved;

			if (testMode)
			{
				gameObject.SetActive(true);
			}
		}

		#endregion

		#region Private Methods

		private void OnClick()
		{
			if (testMode)
			{
				OnRewardAdGranted("", 0);

				return;
			}

			if (MobileAdsManager.Instance.RewardAdState != AdNetworkHandler.AdState.Loaded)
			{
				gameObject.SetActive(false);

				Debug.LogError("[RewardAdButton] The reward button was clicked but there is no ad loaded to show.");

				return;
			}

			MobileAdsManager.Instance.ShowRewardAd(OnRewardAdClosed, OnRewardAdGranted);
		}

		private void OnRewardAdLoaded()
		{
			gameObject.SetActive(true);
		}

		private void OnRewardAdClosed()
		{
			gameObject.SetActive(false);
		}

		private void OnRewardAdGranted(string rewardId, double rewardAmount)
		{
			// Get the current amount of coins
			int animateFromCoins = GameController.Instance.Coins;

			// Give the amount of coins
			GameController.Instance.GiveCoins(coinsToReward, false);

			// Get the amount of coins now after giving them
			int animateToCoins = GameController.Instance.Coins;

			// Show the popup to the user so they know they got the coins
			PopupManager.Instance.Show("reward_ad_granted", new object[] { coinsToReward, animateFromCoins, animateToCoins } );
		}

		private void OnAdsRemoved()
		{
			MobileAdsManager.Instance.OnRewardAdLoaded -= OnRewardAdLoaded;

			gameObject.SetActive(false);
		}

		#endregion
	}
}

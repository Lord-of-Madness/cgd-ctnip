using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenScript : MonoBehaviour
{

	[SerializeField]
	Button loadButton;

	[SerializeField]
	Image gameOverImage;
	[SerializeField]
	float fadeInDuration = 1.0f;

	Tween imgFadeTween;

	private void Awake()
	{
		Hide();
	}

	public void Show()
	{
		if (imgFadeTween.IsActive()) return;

		imgFadeTween = gameOverImage.DOFade(1, fadeInDuration).OnComplete(enableButton).SetUpdate(true);

	}

	public void Hide()
	{
		disableButton();
		gameOverImage.color = new Color(gameOverImage.color.r, gameOverImage.color.g, gameOverImage.color.b, 0);
	}

	void enableButton()
	{
		loadButton.gameObject.SetActive(true);
	}

	void disableButton()
	{
		loadButton.gameObject.SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovAnim : MonoBehaviour
{
	public Transform spriteTransform;
	public Vector3 targetPosition;
	public Color targetColor;
	public float moveDuration = 1f;
	public float colorFadeDuration = 0.5f;
	Coroutine AnimRoutine;

	public void Initialise(Color _targetColor, Vector3 TargetPos, bool IsStartgame)
	{
		this.GetComponent<TrailRenderer>().material.color = _targetColor;
		targetColor = _targetColor;
		targetPosition = TargetPos;
		if (AnimRoutine != null)
		{
			StopCoroutine(AnimRoutine);
			AnimRoutine = StartCoroutine(AnimateSprite(IsStartgame));
		}
		else
		{
			AnimRoutine = StartCoroutine(AnimateSprite(IsStartgame));
		}
	}
	private IEnumerator AnimateSprite(bool IsStartGame)
	{
		// Move sprite to starting position
		if(IsStartGame)
		{
			Vector3 startingPosition = targetPosition + Vector3.up * 2f;
			spriteTransform.position = startingPosition;
		}
		// Move sprite
		LeanTween.move(spriteTransform.gameObject, targetPosition, moveDuration).setEase(LeanTweenType.easeSpring);

		// Fade in sprite color
		if(IsStartGame)
		{
			Color startingColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
			spriteTransform.GetComponent<SpriteRenderer>().color = startingColor;
			LeanTween.alpha(spriteTransform.gameObject, targetColor.a, moveDuration);
		}
		yield return new WaitForSeconds(moveDuration);
	}
}
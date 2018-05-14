using UnityEngine;
using System.Collections;

public class CharacterProperties : MonoBehaviour {
	
	public Vector3 projectileAnimationOffset = new Vector3 (0, 0, 0.3f);
	// All data related to movement.
	public MovementData m_MovementData;
	
	public AnimationClip[] idleAnims;
	public AudioClip[] idleAudio;
	public AnimationClip[] walkForwardAnims;
	public AudioClip[] walkForwardAudio;
	public AnimationClip[] meleeAttackAnims;
	public AudioClip[] meleeAttackAudio;
	public AnimationClip[] rangedAttackAnims;
	public AudioClip[] rangedAttackAudio;
	public AnimationClip[] getHitAnims;
	public AudioClip[] getHitAudio;
	public AnimationClip[] deathAnims;
	public AudioClip[] deathAudio;
}

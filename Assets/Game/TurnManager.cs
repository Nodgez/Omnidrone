using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public enum TurnPhase { Move, Attack, Wrapup }
    public int TurnNumber { get; set; }
}

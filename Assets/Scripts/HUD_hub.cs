using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD_hub : MonoBehaviour
{
    [Header("Speedometer")]
    public TMP_Text speedLabel; // The label that displays the speed;
    public RectTransform speedArrow; // The arrow in the speedometer

    [Header("Tachometer")]
    public TMP_Text rpmLabel;
    public RectTransform rpmArrow; // The arrow in the speedometer

    [Header("Altitude")]
    public RectTransform smallAltArrow;
    public RectTransform altBigArrow;

    [Header("Climb")]
    public RectTransform climbArrow;

    [Header("Weapons-related")]
    public TMP_Text missiles, rockets, bombs;

    [Header("Battle-Related")]
    public TMP_Text battleTime, battleScore;
}

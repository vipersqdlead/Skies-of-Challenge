using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SurvivalSettings;

public class SurvivalUnlockableCheck : MonoBehaviour
{
    [SerializeField] SurvivalSettings settings;
    public PlaneTypes playerPlane;
    public Sprite[] planePNGs;
    public TMP_Text planeText;
    public Image planeImg;


    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F4))
    //    {
    //        PlayerPrefs.SetInt("Unlock All Aircraft", 1);
    //    }
    //    if (Input.GetKeyDown(KeyCode.F3))
    //    {
    //        PlayerPrefs.SetInt("Unlock All Aircraft", 0);
    //    }
    //}
    //public void ChangePlayerPlanesRight()
    //{
    //    if (CheckPlaneAvailability(playerPlane + 1) && (int)playerPlane < Enum.GetValues(typeof(PlaneTypes)).Length - 1)
    //    {
    //        playerPlane++;
    //    }
    //    else if ((int)playerPlane > Enum.GetValues(typeof(PlaneTypes)).Length - 1)
    //    {
    //        playerPlane = 0;

    //        if (!CheckPlaneAvailability(0))
    //        {
    //            ChangePlayerPlanesRight();
    //        }
    //    }
    //    else
    //    {
    //        playerPlane++;
    //        ChangePlayerPlanesRight();
    //    }
    //    planeImg.sprite = planePNGs[(int)playerPlane];
    //    planeText.text = playerPlane.ToString();
    //}

    //public void ChangePlayerPlanesLeft()
    //{
    //    if (CheckPlaneAvailability(playerPlane - 1) && (int)playerPlane > 0)
    //    {
    //        playerPlane--;
    //    }
    //    else if ((int)playerPlane <= 0)
    //    {
    //        int _int = Enum.GetValues(typeof(PlaneTypes)).Length - 1;
    //        playerPlane = (PlaneTypes)_int;
    //        if (!CheckPlaneAvailability((PlaneTypes)_int))
    //        {
    //            ChangePlayerPlanesLeft();
    //        }
    //    }
    //    else
    //    {
    //        playerPlane--;
    //        ChangePlayerPlanesLeft();
    //    }
    //    planeImg.sprite = planePNGs[(int)playerPlane];
    //    planeText.text = playerPlane.ToString();
    //}

    //bool CheckPlaneAvailability(PlaneTypes type)
    //{

    //    if(PlayerPrefs.GetInt("Unlock All Aircraft") == 1)
    //    {
    //        return true;
    //    }

    //    switch (type)
    //    {
    //        case PlaneTypes.Emil:
    //                return true;
    //        case PlaneTypes.Fried:
    //            if (PlayerPrefs.GetInt("Trager Total Kill Count") >= 20)
    //            {  return true; }
    //            else
    //            {   return false; }
    //        case PlaneTypes.TragerLate:
    //            if (PlayerPrefs.GetInt("Trager Total Kill Count") >= 50 && PlayerPrefs.GetInt("Trager Highest Kill Count") >= 25)
    //            {  return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Krahe :
    //            if (PlayerPrefs.GetInt("Trager (Late) Highest Kill Count") >= 25 && PlayerPrefs.GetInt("General Total Score") > 90000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HispanoCobra:
    //            if(PlayerPrefs.GetInt("Airacobra Total Kill Count") > 30)
    //            {  return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.AiracobraL:
    //            if(PlayerPrefs.GetInt("Airacobra Highest Score") >= 30000 || PlayerPrefs.GetInt("Hispanocobra Highest Score") >= 30000)
    //            {  return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.VargA:
    //            if (PlayerPrefs.GetInt("Varg Highest Time Alive") >= 300)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ReisenKai:
    //            if (PlayerPrefs.GetInt("Reisen Total Time Alive") >= 1000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HayabusaIII:
    //            if (PlayerPrefs.GetInt("Reisen-Kai Total Time Alive") + PlayerPrefs.GetInt("Reisen Total Time Alive") >= 1500 && (PlayerPrefs.GetInt("Hien Total Score") + PlayerPrefs.GetInt("Hien-I Total Score") + PlayerPrefs.GetInt("Hien-III-Otsu Total Score") >= 50000))
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.NightHellcat:
    //            if (PlayerPrefs.GetInt("Hellcat Total Time Alive") >= 600 && PlayerPrefs.GetInt("Hellcat Total Kill Count") >= 100)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SuperHellcat:
    //            if (PlayerPrefs.GetInt("Hellcat Total Kill Count") + PlayerPrefs.GetInt("Night Hellcat Total Kill Count") >= 150)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.CorsairB:
    //            if (PlayerPrefs.GetInt("Corsair Total Score") > 100000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SuperCorsair:
    //            if (PlayerPrefs.GetInt("Corsair Total Kill Count") + PlayerPrefs.GetInt("Corsair-B Total Kill Count") >= 300)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Truehawk:
    //            if (PlayerPrefs.GetInt("General Total Fly Time") > 10000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SuperWarhawk:
    //            if (PlayerPrefs.GetInt("Warhawk Total Score") + PlayerPrefs.GetInt("Truehawk Total Score") > 100000 && PlayerPrefs.GetInt("General Total Fly Time") > 20000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Apache:
    //            if (PlayerPrefs.GetInt("Mustang-C Total Kill Count") > 30)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MustangA:
    //            if (PlayerPrefs.GetInt("Mustang-C Highest Score") >= 30000 || PlayerPrefs.GetInt("Apache Highest Score") >= 30000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MustangD:
    //            if (PlayerPrefs.GetInt("Mustang-C Total Time Alive") + PlayerPrefs.GetInt("Mustang-A Total Time Alive") + PlayerPrefs.GetInt("Apache Total Time Alive") >= 2000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SkayatekaMustang:
    //            if (PlayerPrefs.GetInt("Mustang-A Highest Time Alive") >= 1200 || PlayerPrefs.GetInt("Mustang-D Highest Time Alive") >= 1200)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MustangH:
    //            if (PlayerPrefs.GetInt("Mustang-D Total Score") + PlayerPrefs.GetInt("Mustang-A Total Score") + PlayerPrefs.GetInt("Mustang-C Total Score") + PlayerPrefs.GetInt("Apache Total Score") + PlayerPrefs.GetInt("Skayateka Mustang Total Score") > 500000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.YakovlevT:
    //            if (PlayerPrefs.GetInt("Yakovlev Total Time Alive") >= 600)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.YakovlevK:
    //            if (PlayerPrefs.GetInt("Yakovlev-T Highest Score") >= 30000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.YakovlevU:
    //            if (PlayerPrefs.GetInt("General Total Kills") >= 1000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.YakovlevJ:
    //            if (PlayerPrefs.GetInt("General Total Kills") >= 2000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.AntonA:
    //            if (PlayerPrefs.GetInt("Anton Total Kill Count") >= 30)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.AntonHotrod:
    //            if (PlayerPrefs.GetInt("General Total Fly Time") >= 10000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HellWhisper:
    //            if (PlayerPrefs.GetInt("Anton Total Kill Count") + PlayerPrefs.GetInt("Anton-A Total Kill Count") >= 200 && PlayerPrefs.GetInt("Hell's Whisper Times Killed") > 5)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ZerstorerH:
    //            if (PlayerPrefs.GetInt("Zerstorer Total Time Alive") >= 1500 && PlayerPrefs.GetInt("Zerstorer Highest Time Alive") >= 600)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.DinahKai:
    //            if (PlayerPrefs.GetInt("Dinah Highest Score") >= 40000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Raiko:
    //            if (PlayerPrefs.GetInt("Dinah Total Kill Count") + PlayerPrefs.GetInt("Dinah-Kai Total Kill Count") >= 120)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.LightningK:
    //            if (PlayerPrefs.GetInt("Lightning Total Time Alive") >= 2000 && PlayerPrefs.GetInt("Lightning Highest Kill Count") >= 50)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SpitfireII:
    //            if (PlayerPrefs.GetInt("Survival High Score") >= 30000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SpitfireXXIV:
    //            if (PlayerPrefs.GetInt("Survival Highest Kills") >= 120)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.BearcatB:
    //            if (PlayerPrefs.GetInt("Bearcat Total Kill Count") >= 150)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.FalkeB:
    //            if (PlayerPrefs.GetInt("Falke Total Time Alive") >= 3000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.FalkeG:
    //            if (PlayerPrefs.GetInt("Falke-B Highest Kill Count") >= 70 || PlayerPrefs.GetInt("Falke Highest Kill Count") >= 70)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Moskito:
    //            if (PlayerPrefs.GetInt("Falke-B Highest Time Alive") >= 1500 || PlayerPrefs.GetInt("Falke-G Highest Time Alive") >= 1500 || PlayerPrefs.GetInt("Falke Highest Time Alive") >= 1500)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Superbolt:
    //            if (PlayerPrefs.GetInt("General Total Score") >= 1250000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Ruina:
    //            if (PlayerPrefs.GetInt("Ruina Times Killed") >= 5)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Sismo:
    //            if (PlayerPrefs.GetInt("Sismo Times Killed") >= 10)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HienI:
    //            if (PlayerPrefs.GetInt("Hien Highest Time Alive") >= 900)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HienIIIOtsu:
    //            if ((PlayerPrefs.GetInt("Hien Total Time Alive") + (PlayerPrefs.GetInt("Hien-I Total Time Alive")) >= 1500) && PlayerPrefs.GetInt("Hayate Total Time Alive") + PlayerPrefs.GetInt("Hayate Otsu Total Time Alive") + PlayerPrefs.GetInt("Hayate Hei Total Time Alive") + PlayerPrefs.GetInt("Hayate-II Total Time Alive") > 1500)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.TenraiKai:
    //            if (PlayerPrefs.GetInt("Tenrai Highest Time Alive") >= 1000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.TenraiSuperKai:
    //            if (PlayerPrefs.GetInt("Tenrai Highest Score") >= 75000 || PlayerPrefs.GetInt("Tenrai-Kai Highest Kill Count") >= 80)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.LynxD:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 16)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.WhiteWolf: 
    //            if(PlayerPrefs.GetInt("White Wolf Times Killed") >= 3)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Shoki:
    //            if (PlayerPrefs.GetInt("Raiden Highest Time Alive") >= 1200)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.CentauroA:
    //            if (PlayerPrefs.GetInt("Centauro Highest Kill Count") >= 50)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.DolchA:
    //            if (PlayerPrefs.GetInt("Dolch Total Score") + PlayerPrefs.GetInt("Kurfurst Total Score") >= 60000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.DolchD:
    //            if (PlayerPrefs.GetInt("Survival Longest Alive") >= 1000)
    //            { return true; }
    //            else
    //            { return false; }
    //         case PlaneTypes.DolchE:
    //            if (PlayerPrefs.GetInt("Survival Longest Alive") >= 1500)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.XinyiYongnian:
    //            if (PlayerPrefs.GetInt("Xinyi Yongnian Times Killed") >= 5)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MacchiC:
    //            if (PlayerPrefs.GetInt("Macchi Highest Score") >= 50000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MoonbatB:
    //            if (PlayerPrefs.GetInt("Survival Highest Score") >= 80000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MansyuuKai:
    //            if (PlayerPrefs.GetInt("Mansyuu Total Time Alive") >= 1000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Senden:
    //            if (PlayerPrefs.GetInt("Mansyuu Total Score") + PlayerPrefs.GetInt("Mansyuu-Kai Total Score") >= 150000 && (PlayerPrefs.GetInt("Reppu Total Kill Count") + PlayerPrefs.GetInt("Reppu-C Total Kill Count") + PlayerPrefs.GetInt("Reppu-Kai Total Kill Count") >= 100))
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.WhiteFootFox:
    //            if (PlayerPrefs.GetInt("Kurfurst Total Kill Count") >= 158 && PlayerPrefs.GetInt("White-Foot Fox Times Killed") >= 3)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.DoraLate:
    //            if (PlayerPrefs.GetInt("General Total Kills") >= 2500)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.KogarashiI:
    //            if (PlayerPrefs.GetInt("Kogarashi Highest Score") >= 40000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.KogarashiOtsu:
    //            if (PlayerPrefs.GetInt("Kogarashi Highest Kill Count") >= 60 || PlayerPrefs.GetInt("Kogarashi-I Highest Kill Count") >= 60 && PlayerPrefs.GetInt("Hayate Total Time Alive") + PlayerPrefs.GetInt("Hayate Otsu Total Time Alive") + PlayerPrefs.GetInt("Hayate Hei Total Time Alive") + PlayerPrefs.GetInt("Hayate-II Total Time Alive") > 1800)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.TankC:
    //            if (PlayerPrefs.GetInt("Tank Total Score") >= 120000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Kyofu:
    //            if (PlayerPrefs.GetInt("General Total Fly Time") >= 25000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ShidenKaiIV:
    //            if (PlayerPrefs.GetInt("Shiden-Kai Highest Score") >= 50000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.PyorremyrskyLate:
    //            if (PlayerPrefs.GetInt("Pyorremyrsky Total Kill Count") >= 80)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MathiasFleisher:
    //            if (PlayerPrefs.GetInt("Mathias Fleisher Times Killes") >= 5 && (PlayerPrefs.GetInt("Pyorremyrsky Highest Kill Count") >= 50 || PlayerPrefs.GetInt("Pyorremyrsky (Late) Highest Kill Count") >= 50))
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Ghost:
    //            if ((PlayerPrefs.GetInt("Ghost Times Killed") >= 20 && PlayerPrefs.GetInt("General Total Fly Time") > 10000000) || PlayerPrefs.GetInt("Survival High Score") > 500000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Assassin:
    //            if (PlayerPrefs.GetInt("Assassin Times Killed") >= 10 && PlayerPrefs.GetInt("General Total Kills") >= 10000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ReppuC:
    //            if (PlayerPrefs.GetInt("Reppu Highest Kill Count") >= 75)
    //            { return true;}
    //            else
    //            { return false; }
    //        case PlaneTypes.ReppuKai:
    //            if (PlayerPrefs.GetInt("Reppu Total Score") + PlayerPrefs.GetInt("Reppu-C Total Score") >= 200000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Crusader:
    //            if (PlayerPrefs.GetInt("Crusader Nagao Times Killed") >= 5 && PlayerPrefs.GetInt("Blue Angel Times Killed") >= 5)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HayateOtsu:
    //            if (PlayerPrefs.GetInt("Hayate Total Time Alive") >= 2000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HayateHei:
    //            if (PlayerPrefs.GetInt("Hayate Highest Score") >= 50000 || PlayerPrefs.GetInt("Hayate Highest Score") >= 65000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HayateII:
    //            if (PlayerPrefs.GetInt("Hayate Total Time Alive") + PlayerPrefs.GetInt("Hayate Otsu Total Time Alive") + PlayerPrefs.GetInt("Hayate Hei Total Time Alive") >= 3000 && PlayerPrefs.GetInt("Blue Angel Times Killed") >= 2 && PlayerPrefs.GetInt("Survival Highest Round") >= 15)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.BlueAngel:
    //            if (PlayerPrefs.GetInt("Blue Angel Times Killed") >= 10)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ShindenLate:
    //            if (PlayerPrefs.GetInt("General Total Score") >= 2500000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Reaper:
    //            if (PlayerPrefs.GetInt("The Reaper Times Killed") >= 50)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SeiranEarly:
    //            if (PlayerPrefs.GetInt("Seiran Total Score") >= 130000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SeiranKai:
    //            if (PlayerPrefs.GetInt("Seiran Total Score") + PlayerPrefs.GetInt("Seiran (Early) Total Score") >= 200000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Cheetah:
    //            if (PlayerPrefs.GetInt("Tigercat Total Time Alive") >= 1500)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.TigercatC:
    //            if (PlayerPrefs.GetInt("Tigercat Total Kill Count") + PlayerPrefs.GetInt("Cheetah Total Kill Count") >= 120)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ZwillingK:
    //            if (PlayerPrefs.GetInt("General Total Kills") >= 4000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.TakaKai:
    //            if (PlayerPrefs.GetInt("Survival Highest Kills") >= 120)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SchwalbeIb:
    //            if (PlayerPrefs.GetInt("Schwalbe Total Kill Count") >= 50)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SchwalbeIc:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.BlitzB:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.KikkaOtsu:
    //            if (PlayerPrefs.GetInt("Reisen Total Score") + PlayerPrefs.GetInt("Reisen-Kai Total Score") + PlayerPrefs.GetInt("Raiden Total Score") + PlayerPrefs.GetInt("Tenrai Total Score") + PlayerPrefs.GetInt("Tenrai-Kai Total Score") + PlayerPrefs.GetInt("Seiran (Early) Total Score") + PlayerPrefs.GetInt("Tenrai-SuperKai Total Score") + PlayerPrefs.GetInt("Shiden-Kai-IV Total Score") + PlayerPrefs.GetInt("Reppu Total Score") + PlayerPrefs.GetInt("Reppu-C Total Score") + PlayerPrefs.GetInt("Reppu-Kai Total Score") + PlayerPrefs.GetInt("Seiran Total Score") + PlayerPrefs.GetInt("Seiran (Early) Total Score") + PlayerPrefs.GetInt("Seiran-Kai Total Score") + PlayerPrefs.GetInt("Kikka Total Score") >= 1500000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ShootingStarC:
    //            if (PlayerPrefs.GetInt("Shooting Star Total Kill Count") >= 70)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ShootingStarT:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Starfire:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.FargoLate:
    //            if (PlayerPrefs.GetInt("Fargo Highest Time Alive") >= 900f)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SeaVenom:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SeaVixen:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.GinaPreserie:
    //            if (PlayerPrefs.GetInt("Gina Highest Kills") >= 30)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.KaryuKai:
    //            if (PlayerPrefs.GetInt("Campaign Clear") >= 1)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.DrachentoterB:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Gunval:
    //            if (PlayerPrefs.GetInt("Sabre Total Time Alive") >= 1500)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.FJ2Fury:
    //            if (PlayerPrefs.GetInt("Gunval Highest Kill Count") >= 35)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.CACSabre:
    //            if (PlayerPrefs.GetInt("Sabre Total Score") + PlayerPrefs.GetInt("Gunval Total Score") + PlayerPrefs.GetInt("FJ2 Fury Total Score") >= 650000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.WhiteDaze:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Fresco:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.TunnanD:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SchwalbeIIIb:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Cougar:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Jaguar:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SkyhawkM:
    //            if (PlayerPrefs.GetInt("Skyhawk Highest Kill Count") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Ayit:
    //            if (PlayerPrefs.GetInt("Skyhawk-M Highest Time Alive") >= 900)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.FightingHawk:
    //            if (PlayerPrefs.GetInt("Skyhawk-M Highest Kill Count") >= 35)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HatsutakaT:
    //            if(PlayerPrefs.GetInt("Hatsutaka Highest Score") >= 50000)
    //            { return true; }
    //            else 
    //            { return false; }
    //        case PlaneTypes.EnteB:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.HarrierC:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.CrusaderE:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.KazeLate:
    //            if (PlayerPrefs.GetInt("General Total Kills") >= 2000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.DrakenJ:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Cipher:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Kfir:
    //            if (PlayerPrefs.GetInt("Mirage III Total Time Alive") >= 1000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.WhiteDazeII:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Bison:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.FreedomFighter:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Yuurei:
    //            if (PlayerPrefs.GetInt("Phantom II Highest Score") >= 30000 && (PlayerPrefs.GetInt("Kaze Total Kill Count") + PlayerPrefs.GetInt("Kaze (Late) Total Kill Count") >= 30))
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.Fengren:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.ViggenAJS:
    //            if (PlayerPrefs.GetInt("Survival Highest Round") >= 25)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.SuperDeltaDart:
    //            if (PlayerPrefs.GetInt("Delta Dart Highest Score") >= 30000)
    //            { return true; }
    //            else
    //            { return false; }
    //        case PlaneTypes.MirageG8:
    //            if (PlayerPrefs.GetInt("Mirage F1 Highest Time Alive") >= 600)
    //            { return true; }
    //            else
    //            { return false; }
    //    }
    //    return false;
    //}

    //public void StartBattle()
    //{
    //    int selection = (Enum.GetValues(typeof(PlaneTypes)).Length - 1) + ((int)playerPlane + 1);
    //    print(selection);
    //    settings.SaveChanges(selection);
    //    StartCoroutine(settings.ToBattle());
    //}

    //public enum SpPlaneTypes
    //{
    //    Emil,
    //    Fried,
    //    TragerLate,
    //    Krah ,
    //    HispanoCobra,
    //    AiracobraL,
    //    LightVarg,
    //    ReisenKai,
    //    Hayabusa,
    //    NightHellcat,
    //    SuperHellcat,
    //    CorsairB,
    //    SuperCorsair,
    //    Truehawk,
    //    SuperWarhawk,
    //    Apache,
    //    MustangA,
    //    MustangD,
    //    SkayatekaMustang,
    //    MustangH,
    //    YakovlevT,
    //    YakovlevK,
    //    YakovlevU,
    //    YakovlevJ,
    //    AntonA,
    //    AntonHotrod,
    //    HellWhisper,
    //    ZerstorerH,
    //    DinahKai,
    //    Raiko,
    //    LightningK,
    //    SpitfireII,
    //    SpitfireXXIV,
    //    BearcatB,
    //    FalkeB,
    //    FalkeG,
    //    Moskito,
    //    Superbolt,
    //    Ruina,
    //    Sismo,
    //    HienI,
    //    HienIIIOtsu,
    //    TenraiKai,
    //    TenraiSuperKai,
    //    LynxD,
    //    WhiteWolf,
    //    Shoki,
    //    CentauroA,
	   // Airacomet,
    //    DolchA,
    //    DolchD,
	   // DolchE,
    //    XinyiYongnian,
    //    MacchiC,
    //    MoonbatB,
    //    MansyuuKai,
    //    Senden,
    //    WhiteFootFox,
    //    DoraLate,
    //    KogarashiI,
    //    KogarashiOtsu,
    //    TankC,
    //    Kyofu,
    //    ShidenKaiIV,
    //    PyorremyrskyLate,
    //    MathiasFleisher,
    //    Ghost,
    //    Assassin,
    //    ReppuC,
    //    ReppuKai,
    //    Crusader,
    //    HayateOtsu,
    //    HayateHei,
    //    HayateII,
    //    BlueAngel,
    //    ShindenLate,
    //    Reaper,
    //    SeiranEarly,
    //    SeiranKai,
    //    Cheetah,
    //    TigercatC,
    //    ZwillingK,
    //    TakaKai,
    //    SchwalbeIb,
    //    SchwalbeIc,
    //    BlitzB,
    //    KikkaOtsu,
    //    ShootingStarC,
    //    ShootingStarT,
    //    Starfire,
    //    FargoLate,
    //    SeaVenom,
    //    SeaVixen,
    //    GinaPreserie,
    //    KaryuKai,
    //    DrachentoterB,
    //    Gunval,
    //    FJ2Fury,
    //    CACSabre,
    //    WhiteDaze,
    //    Fresco,
    //    TunnanD,
    //    SchwalbeIIIb,
    //    Cougar,
    //    Jaguar,
    //    SkyhawkM,
    //    Ayit,
    //    FightingHawk,
    //    HatsutakaT,
    //    EnteB,
    //    HarrierC,
    //    CrusaderC,
    //    KazeLate,
    //    DrakenJ,
    //    Cipher,
    //    Kfir,
    //    WhiteDazeII,
    //    Bison,
    //    FreedomFighter,
    //    Yuurei,
    //    FloggerD,
    //    ViggenAJS,
    //    SuperDeltaDart,
    //    MirageG8
    //}
}

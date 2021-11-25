using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;

/// <summary>
/// 数据准备：
/// 通过SimpleJson解析json格式的数据，存储在List类型的 RankListModels 中
/// </summary>
public class GetRankListModels
{
    public static List<RankListModel> RankListModels = new List<RankListModel>();//存储得到的Json数据
    public static double countDown;//倒计时的秒数时间戳

    //通过SimpleJson解析，得到商品数据
    public void Awake()
    {
        //获得数据文件路径下的text格式数据
        TextAsset filePath = (TextAsset)Resources.Load("JsonData/ranklist");
        //转为Json格式
        JSONNode jsonNode = JSON.Parse(filePath.text);
        countDown = jsonNode[0];
        //取出第一个数据的值 即：jsonNode[0]，里面存有所有商品信息
        JSONNode jsonNode1 = jsonNode[1];

        //循环遍历json数据，并映射到实体类DailyProduct中
        for (int  i = 0; i < jsonNode1.Count; i++)
        {
            //实例化一个商品对象，用于映射json数据
            RankListModel rankListModel = new RankListModel();
            rankListModel.Uid = jsonNode1[i]["uid"];
            rankListModel.NickName = jsonNode1[i]["nickName"];
            rankListModel.Avatar = jsonNode1[i]["avatar"];
            rankListModel.Trophy = jsonNode1[i]["trophy"];
            
            //通过数组存储数据
            RankListModels.Add(rankListModel);
        }
        
        //排序：排序规则在实体类 RankListModel 中的 CompareTo（）方法
        RankListModels.Sort();
    }
    
}
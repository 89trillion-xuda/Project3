using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using UnityEngine.UIElements;
using RankData;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;


// 您应该将名称空间修改为您自己的名称，或者 - 如果您确定不会发生冲突 - 将其完全删除
namespace BasicListAdapter
{
	// 除了 Start() 之外，您还需要实现 1 个重要的回调：UpdateCellViewsHolder()
	// 看下面的解释
	public class BasicListAdapter : OSA<BaseParamsWithPrefab, MyListItemViewsHolder>
	{
		// 存储数据并在项目计数发生变化时通知适配器的助手
		// 可以迭代，也可以通过 [] 操作符访问其元素
		public SimpleDataHelper<MyListItemModel> Data { get; private set; }
		
		//前三名的背景
		[SerializeField] private Sprite normalRankBg;
		[SerializeField] private Sprite rank3Bg;
		[SerializeField] private Sprite rank2Bg;
		[SerializeField] private Sprite rank1Bg;

		//前三名的奖牌 
		[SerializeField] private Sprite rank1;
		[SerializeField] private Sprite rank2;
		[SerializeField] private Sprite rank3;
		
		//前三名 和 普通的 的头像边框
		[SerializeField] private Sprite BorderImg1;
		[SerializeField] private Sprite BorderImg2;
		[SerializeField] private Sprite BorderImg3;
		[SerializeField] private Sprite BorderImg4;

		//声明段位图标
		[SerializeField] private Sprite AreanBadge1;
		[SerializeField] private Sprite AreanBadge2;
		[SerializeField] private Sprite AreanBadge3;
		[SerializeField] private Sprite AreanBadge4;
		[SerializeField] private Sprite AreanBadge5;
		[SerializeField] private Sprite AreanBadge6;
		[SerializeField] private Sprite AreanBadge7;
		[SerializeField] private Sprite AreanBadge8;
		
		//获得弹窗
		[SerializeField] private GameObject PopUpsObjClone;
		//弹窗里显示的名字
		[SerializeField] private Text UserNameTxtClone;
		//弹窗里显示的排名
		[SerializeField] private Text RankNumTxtClone;
		
		//获得倒计时时间的相应字段
		[SerializeField] private Text RemainDay;
		[SerializeField] private Text RemainHours;
		[SerializeField] private Text RemainMinute;
		[SerializeField] private Text RemainSecond;
		
		//获得Banner中自己的排名
		[SerializeField] private Text MyRankTxt;
		//获得Banner中自己的排名图片
		[SerializeField] private Image MyRankImg;
		//获得Banner中自己的头像边框
		[SerializeField] private Image MyBorderImg;
		//获得Banner自己的名称
		[SerializeField] private Text MyNameTxt;
		//获得Banner中自己的奖杯数
		[SerializeField] private Text MyTrophyTxt;


		#region OSA implementation
		protected override void Awake()
		{
			Data = new SimpleDataHelper<MyListItemModel>(this);

			// 调用它初始化内部数据并准备适配器以处理项目计数变化
			base.Awake();
			
			//启动计算倒计时的协程
			StartCoroutine(Time());

			//实例化获得Json数据的类
			var getRankListModels = new GetRankListModels();
			//调用Awake（）方法，初始化获得数据
			getRankListModels.Awake();
			//获得Json数据
			List<RankListModel> rankListModels = GetRankListModels.RankListModels;

			// 从您的数据源中检索模型并设置项目数
			RetrieveDataAndUpdate(rankListModels.Count);
		}
		
		IEnumerator Time()
		{
			//循环
			while (true)
			{
				//一秒后再执行一次
				yield return new WaitForSeconds(1);

				//倒计时为0的时候退出循环，不再更新倒计时
				if (GetRankListModels.countDown == 0)
				{
					break;
				}
				
				//标准时间
				DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
				//相差的时间戳转日期时间
				DateTime over = startTime.AddSeconds(GetRankListModels.countDown);
				//将相应的倒计时时间字段显示在视图上
				RemainDay.text = over.Day.ToString();
				RemainHours.text = over.Hour.ToString();
				RemainMinute.text = over.Minute.ToString();
				RemainSecond.text = over.Second.ToString();

				//时间戳--
				GetRankListModels.countDown--;
			}
		}

		// 任何时候之前不可见的项目变得可见时都会调用它，或者在它被创建之后，
		// 或者当任何需要刷新的事情发生时
		// 在这里，您将模型中的数据绑定到项目的视图
		// *对于方法的完整描述，请检查基本实现
		protected override MyListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyListItemViewsHolder();

			// 使用这个快捷方式可以避免：
			// - 自己实例化预制件
			// - 启用实例游戏对象
			// - 设置它的索引
			// - 调用它的 CollectViews()
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		// 任何时候之前不可见的项目变得可见时都会调用它，或者在它被创建之后，
		// 或者当任何需要刷新的事情发生时
		// 在这里，您将模型中的数据绑定到项目的视图
		// *对于方法的完整描述，请检查基本实现
		protected override void UpdateViewsHolder(MyListItemViewsHolder newOrRecycled)
		{
			// 在此回调中，“newOrRecycled.ItemIndex” 保证始终反映
			// 应由此视图持有者表示的项目的索引。您将使用此索引
			// 从你的数据集中检索模型
			MyListItemModel model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.TrophyNumTxt.text = model.TrophyNum.ToString();
			newOrRecycled.NickNameTxt.text = model.NickNameTxt.ToString();
			if (model.RankTxt <= 3)
			{
				//前三名显示特定排名图片和头像边框，不显示数字排名
				newOrRecycled.RankImg.sprite = model.RankImg;
				newOrRecycled.BorderImg.sprite = model.BorderImg;
				newOrRecycled.RankImg.gameObject.SetActive(true);
				//防止资源图片变形
				newOrRecycled.RankImg.SetNativeSize();
			}
			else
			{
				//其他人显示数字排名，没有特定排名图片和头像边框
				newOrRecycled.RankImg.gameObject.SetActive(false);
				newOrRecycled.BorderImg.sprite = BorderImg4;
				newOrRecycled.RankTxt.gameObject.SetActive(true);
				newOrRecycled.RankTxt.text = model.RankTxt.ToString();
			}

			//每条排行榜添加点击事件
			newOrRecycled.RankListBtn.GetComponent<Button>().onClick.AddListener(() =>
			{
				//显示对话框
				PopUpsObjClone.SetActive(true);
				//显示自己的名字
				UserNameTxtClone.text = model.NickNameTxt;
				//显示自己的奖杯数
				RankNumTxtClone.text = model.RankTxt.ToString();
			});
			//背景图片
			newOrRecycled.RankListBtnBg.sprite = model.RankListBtnBg;
			//段位等级图片
			newOrRecycled.AreanBadgeImg.sprite = model.AreanBadge;
		}
		#endregion

		#region data manipulation
		public void AddItemsAt(int index, IList<MyListItemModel> items)
		{
			Data.InsertItems(index, items);
		}

		public void RemoveItemsFrom(int index, int count)
		{
			Data.RemoveItems(index, count);
		}

		public void SetItems(IList<MyListItemModel> items)
		{
			Data.ResetItems(items);
		}
		#endregion


		// 将数据数量调进携程函数
		void RetrieveDataAndUpdate(int count)
		{
			// 开启携程
			StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
		}
		
		// 从数据源中检索 <count> 个模型，然后调用 OnDataRetrieved。
		// 在实际情况下，您将查询服务器、数据库或任何数据源，然后调用 OnDataRetrieved
		IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
		{
			// 模拟数据检索延迟
			yield return new WaitForSeconds(.5f);
			var newItems = new MyListItemModel[count];
			
			//获得Json数据
			List<RankListModel> rankListModels = GetRankListModels.RankListModels;

			// 在这里检索您的数据
			for (int i = 0; i < count; ++i)
			{
				//调用接口方法，实现根据每一个List中的数据设置相应的Model信息，并接收返回的model
				MyListItemModel model = setModel(rankListModels[i], i);

				//存入model数组
				newItems[i] = model;

				//设置标头Banner中自己的信息
				if (rankListModels[i].Uid == 3716954261)
				{
					//调用接口方法，传入相关参数，设置Banner信息
					setBanner(i, rankListModels[i].NickName, rankListModels[i].Trophy.ToString());
				}
				
			}

			OnDataRetrieved(newItems);
		}

		void OnDataRetrieved(MyListItemModel[] newItems)
		{
			Data.InsertItemsAtEnd(newItems);
		}

		//根据List中的数据，设置每一个Model的信息，并返回设置好的model
		public MyListItemModel setModel(RankListModel rankListModel, int i)
		{
			var model = new MyListItemModel()
			{
				//名子
				NickNameTxt = rankListModel.NickName,
				TrophyNum = rankListModel.Trophy
			};
			
			//根据奖杯数量判断显示段位
			int num = rankListModel.Trophy / 1000;
			switch (num)
			{
				case 0:
					model.AreanBadge = AreanBadge1;
					break;
				case 1:
					model.AreanBadge = AreanBadge2;
					break;
				case 2:
					model.AreanBadge = AreanBadge3;
					break;
				case 3:
					model.AreanBadge = AreanBadge4;
					break;
				case 4:
					model.AreanBadge = AreanBadge5;
					break;
				case 5:
					model.AreanBadge = AreanBadge6;
					break;
				case 6:
					model.AreanBadge = AreanBadge7;
					break;
				default:
					model.AreanBadge = AreanBadge8;
					break;
			}

			//前三名设置特定的背景、排名图片和头像边框，数字排名为i
			if (i == 0)
			{
				//设置特定背景
				model.RankListBtnBg = rank1Bg;
				//设置特定排名图片
				model.RankImg = rank1;
				//设置特定的头像边框
				model.BorderImg = BorderImg1;
				//设置数字排名
				model.RankTxt = i+1;
			}
			else if (i == 1)
			{
				model.RankListBtnBg = rank2Bg;
				model.RankImg = rank2;
				model.BorderImg = BorderImg2;
				model.RankTxt = i+1;
			}
			else if (i == 2)
			{
				model.RankListBtnBg = rank3Bg;
				model.RankImg = rank3;
				model.BorderImg = BorderImg3;
				model.RankTxt = i+1;
			}
			else
			{
				//其他人显示数字排名
				model.RankTxt = i+1;
				//其他人统一背景颜色
				model.RankListBtnBg = normalRankBg;
			}

			return model;
		}

		//设置Banner中自己信息的接口方法
		public void setBanner(int num, string name, string trophy)
		{
			//根据排名设置自己的排名图片和头像边框
			MyRankImg.gameObject.SetActive(true);
			if (num == 0)
			{
				MyRankImg.sprite = rank1;
				MyBorderImg.sprite = BorderImg1;
			}else if (num == 1)
			{
				MyRankImg.sprite = rank2;
				MyBorderImg.sprite = BorderImg2;
			}else if (num == 2)
			{
				MyRankImg.sprite = rank3;
				MyBorderImg.sprite = BorderImg3;
			}
			else
			{
				MyRankImg.gameObject.SetActive(false);
				MyRankTxt.gameObject.SetActive(true);
			}
			//设置自己的排名文字
			MyRankTxt.text = (num + 1).ToString();
			//设置自己的名字
			MyNameTxt.text = name;
			//设置自己的奖杯数
			MyTrophyTxt.text = trophy;
		}
	}

	// Class containing the data associated with an item
	public class MyListItemModel
	{
		//声明匹配的ui组件内容类型
		//Item背景
		public Sprite RankListBtnBg;
		//头像边框
		public Sprite BorderImg;
		//段位
		public Sprite AreanBadge;
		//奖杯数量
		public int TrophyNum;
		//排名数
		public Sprite RankImg;
		public int RankTxt;
		//排名者名字
		public string NickNameTxt;
	}
	
	public class MyListItemViewsHolder : BaseItemViewsHolder
	{
		//声明用到的UI组件
		public Button RankListBtn;
		public Image RankListBtnBg;
		public Image BorderImg;
		public Image AreanBadgeImg;
		public Text TrophyNumTxt;
		public Image RankImg;
		public Text RankTxt;
		public Text NickNameTxt;
		
		// 从项目的根游戏对象中检索视图
		public override void CollectViews()
		{
			base.CollectViews();
		
			// GetComponentAtPath 是 frame8.Logic.Misc.Other.Extensions 的一个方便的扩展方法
			// 它从变量的类型推断变量的组件，所以你不需要自己指定它
			root.GetComponentAtPath("RankListBtn",out RankListBtn);
			root.GetComponentAtPath("RankListBtn/RankListBtnBg", out RankListBtnBg);
			root.GetComponentAtPath("RankListBtn/RankListObject/BorderImg", out BorderImg);
			root.GetComponentAtPath("RankListBtn/RankListObject/AreanBadgeImg", out AreanBadgeImg);
			root.GetComponentAtPath("RankListBtn/RankListObject/ShadowBgImg/TrophyNumTxt", out TrophyNumTxt);
			root.GetComponentAtPath("RankListBtn/RankListObject/RankImg", out RankImg);
			root.GetComponentAtPath("RankListBtn/RankListObject/RankTxt", out RankTxt);
			root.GetComponentAtPath("RankListBtn/RankListObject/NickNameTxt", out NickNameTxt);
		}
	}
}

# homework

### 20337259 叶泽霖

- 由于github文件大小限制的原因以及其他原因文件一直提交不上去，因此采用了压缩包加报告的方式来进行作业的提交

### 代码实现

OnGUI函数，用于主要的界面显示以及游戏逻辑

```c#
void OnGUI()
	{
		if (GUI.Button(new Rect(400, 0, 150, 75), "Restart！"))
			Init();
		if (result == 0 && GUI.Button(new Rect(400, 75, 150, 75), "Undo！"))
			Undo();
		GUIStyle fontStyle_label = new GUIStyle();
		fontStyle_label.normal.background = null;
		fontStyle_label.normal.textColor = new Color(1, 0, 1);
		fontStyle_label.fontSize = 25;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; ++j)
			{
				if (GUI.Button(new Rect(100 * i, 100 * j, 100, 100), img3) )
				{
					if(result == 0 && order < 9)
                    {
						chessboard_matrix[i, j] = 1 + order % 2;
						undo_matrix[0] = i;		//将该步记录下来以用于悔棋
						undo_matrix[1] = j;
						result = WinnerCheck();	//检查是否胜出
						order++;	//切换到下名玩家
					}
				}
				if (chessboard_matrix[i, j] == 1)
					GUI.Button(new Rect(100 * i, 100 * j, 100, 100), img1);
				else if (chessboard_matrix[i, j] == 2)
					GUI.Button(new Rect(100 * i, 100 * j, 100, 100), img2);
			}
		}
		if (result == 1)
			GUI.Label(new Rect(400, 150, 150, 75), "Player1 wins!", fontStyle_label);
		else if (result == 2)
			GUI.Label(new Rect(400, 150, 150, 75), "Player2 wins!", fontStyle_label);
		else if (order == 9)
			GUI.Label(new Rect(400, 150, 150, 75), "Draw!", fontStyle_label);
		else
			GUI.Label(new Rect(400, 150, 150, 75), "Playing...", fontStyle_label);
	}
```

- 初始化操作

```C#
void Init()	
	{
		order = 0;
		result = 0;
		chessboard_matrix = new int[3, 3]
		{
			{0,0,0}, {0,0,0}, {0,0,0}
		};
		undo_matrix = new int[2] {0, 0};
	}
```

- 载入图片

```c#
private void LoadFromFile(string path, string _name1, string _name2)	//读取图片然后存到Texture2D对象当中
	{
		img1 = new Texture2D(1, 1);
		img1.LoadImage(ReadPNG(path + _name1));
		img2 = new Texture2D(1, 1);
		img2.LoadImage(ReadPNG(path + _name2));
	}

	private byte[] ReadPNG(string path)	//在文件中读取图片
	{
		FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read);
		fileStream.Seek(0, SeekOrigin.Begin);
		byte[] binary = new byte[fileStream.Length]; //创建文件长度的buffer
		fileStream.Read(binary, 0, (int)fileStream.Length);
		fileStream.Close();
		fileStream.Dispose();
		fileStream = null;
		return binary;
	}
```

### 结果

- 游戏开始

![](.\pics1.png)

- 游戏中

![](.\pics2.png)

- 一方胜利

![](.\pics3.png)

- 打平

![](.\pics4.png)

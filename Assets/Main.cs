using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
    GameObject s_gOneStar, s_gTwoStar, s_gThreeStar, s_gFourStar, s_gFiveStar;  //声明5个不同颜色的星星对象
    GameObject[] s_gStar;   //存放5个不同颜色星星的数组
    GameObject[,] gOneStar; //存放生成的星星的二维数组    
    Vector3[,] worldPoint;  //世界坐标数组
    int iScore;             //得分
    int iToScore = 4800;    //目标分数
    int iRow = 14;          //星星的行数
    int iCol = 10;          //星星的列数
    int iKillNum = 2;       //消除的星星数
    //Vector3 zeroPosition;
    GameObject gParticle;
    float x, y;
    ArrayList starArray = new ArrayList(); 

    //粒子特效
    //void particleFunc(float x,float y){
    //    gParticle.particleEmitter.transform.position.Set(x, y, -2);
    //    gParticle.particleEmitter.maxSize = 1f;
    //    gParticle.particleEmitter.maxEnergy = 3;
    //    gParticle.particleEmitter.maxEmission = 30;
    //    gParticle.particleEmitter.minSize = 0.5f;
    //    gParticle.particleEmitter.minEnergy = 1;
    //    gParticle.particleEmitter.minEmission = 10;                      
    
    //}
    //检测死局
    void isDead(int iKillNum){       
        bool bRemap = true;
        ArrayList gRemap = new ArrayList();        
        for (int i = 0; i < iCol; i++)
        {
            for (int j = 0; j < iRow; j++)
            {
                starArray.Clear();
                if (gOneStar[i, j])
                {
                    gRemap.Add(gOneStar[i, j]);
                    oneStarFourSide(i, j);
                    if (starArray.Count > iKillNum-1)
                    {
                        bRemap = false;
                        break;
                    }
                }

            }
        }
        bool bSameColor = false;
        for(int i=0;i<gRemap.Count;i++){
            int iSameColor = 0;
            for(int j=i+1;j<gRemap.Count;j++){
                if(((GameObject)gRemap[i]).name==((GameObject)gRemap[j]).name){                    
                    iSameColor++;                   
                    if (iSameColor > iKillNum-2){
                        bSameColor = true;                        
                        break;
                    }                                     
                }
            }            
        }        
        if (bSameColor && bRemap)
        {            
            for (int i = 0; i < gRemap.Count; i++)
            {
                int randNum = Random.Range(i, gRemap.Count);
                ArrayList aTemp = new ArrayList();
                aTemp.Add(gRemap[i]);
                gRemap[i] = gRemap[randNum];
                gRemap[randNum] = aTemp[0];
            }
            int index = 0;
            for (int i = 0; i < iCol; i++)
            {
                for (int j = 0; j < iRow; j++)
                {
                    if (gOneStar[i, j])
                    {
                        Destroy(gOneStar[i, j]);
                        gOneStar[i, j] = (GameObject)Instantiate((GameObject)gRemap[index], worldPoint[i, j], ((GameObject)gRemap[index]).transform.rotation);
                        gOneStar[i, j].transform.name = ((GameObject)gRemap[index]).name;                        
                        index++;
                    }
                }
            }
        }
    }
    //检测空列函数
    void isEmptyCol(){
        for(int i=8;i>=0;i--){
            if (gOneStar[i, 0])
                continue;//如果不为空，继续下一列的检测
            else { 
                for(int j=i;j<iCol-1;j++){
                    
                        for (int k = 0; k < iRow; k++)
                        {
                            if (gOneStar[j + 1, k])
                            {
                                //Debug.Log("j="+j);
                                gOneStar[j, k] = (GameObject)Instantiate(gOneStar[j + 1, k], worldPoint[j, k], gOneStar[j + 1, k].transform.rotation);//将右边一列依次向左移
                                gOneStar[j, k].transform.name = gOneStar[j + 1, k].name;//重命名
                                GameObject.DestroyImmediate(gOneStar[j + 1, k]);//删除源对象
                             }
                        }                    
                }                
            }                                                  
        }
    }
    //消除方块函数
    void destroy(int i,int j) {        
            if (gOneStar[i, j])
            {                
                GameObject.DestroyImmediate(gOneStar[i, j]);//使用立即删除函数                
            }        

            for (int k = j; k + 1 < iRow; k++)
            {
                if (gOneStar[i, k + 1])
                {
                    gOneStar[i, k] = (GameObject)Instantiate(gOneStar[i, k + 1], worldPoint[i, k], gOneStar[i, k + 1].transform.rotation);//将每一个从上往下复制                    
                    gOneStar[i, k].transform.name = gOneStar[i, k + 1].name;//重命名
                    GameObject.DestroyImmediate(gOneStar[i, k + 1]);//复制之后删除源对象                    
                }
            }             
    }
    //检测同色方块函数
    void oneStarFourSide(int col, int row)
    {        
        bool bRAdd = false;
        bool bLAdd = false;
        bool bUAdd = false;
        bool bDAdd = false;
        if (col < iCol-1 && col >= 0 && row <= iRow-1 && row >= 0)
        {
            if (gOneStar[col + 1, row] != null && gOneStar[col, row].name == gOneStar[col + 1, row].name)
            {
                bool bAdd = true;
                for (int i = 0; i < starArray.Count; i++)
                {
                    if (((Vector2)starArray[i]) == new Vector2(col + 1, row))
                    {
                        bAdd = false;
                        bRAdd = false;
                        break;
                    }
                }
                if (bAdd)
                {
                    starArray.Add(new Vector2(col + 1, row));//如果检测的方块没有加入数组（即之前没有检测过），就将其加入已检测方块数组
                    bRAdd = true;  //递归条件                  
                }
            }
        }

        if (col > 0 && col <= iCol && row >= 0 && row <= iRow-1)
        {
            if (gOneStar[col - 1, row] != null && gOneStar[col, row].name == gOneStar[col - 1, row].name)
            {
                bool bAdd = true;
                for (int i = 0; i < starArray.Count; i++)
                {
                    if (((Vector2)starArray[i]) == new Vector2(col - 1, row))
                    {
                        bAdd = false;
                        bLAdd = false;
                        break;
                    }
                }
                if (bAdd)
                {
                    starArray.Add(new Vector2(col - 1, row));//如果检测的方块没有加入数组（即之前没有检测过），就将其加入已检测方块数组
                    bLAdd = true;  //递归条件                  
                }
            }
        }

        if (row < iRow-1 && row >= 0 && col >= 0 && col <= iCol-1)
        {
            if (gOneStar[col, row + 1] != null && gOneStar[col, row].name == gOneStar[col, row + 1].name)
            {
                bool bAdd = true;
                for (int i = 0; i < starArray.Count; i++)
                {
                    if (((Vector2)starArray[i]) == new Vector2(col, row + 1))
                    {
                        bAdd = false;
                        bUAdd = false;
                        break;
                    }
                }
                if (bAdd)
                {
                    starArray.Add(new Vector2(col, row + 1));//如果检测的方块没有加入数组（即之前没有检测过），就将其加入已检测方块数组
                    bUAdd = true;//递归条件
                }
            }
        }


        if (row > 0 && row <= iRow-1 && col >= 0 && col <= iCol-1)
        {
            if (gOneStar[col, row - 1] != null && gOneStar[col, row].name == gOneStar[col, row - 1].name)
            {
                bool bAdd = true;
                for (int i = 0; i < starArray.Count; i++)
                {
                    if (((Vector2)starArray[i]) == new Vector2(col, row - 1))
                    {
                        bAdd = false;
                        bDAdd = false;
                        break;
                    }
                }
                if (bAdd)
                {
                    starArray.Add(new Vector2(col, row - 1));//如果检测的方块没有加入数组（即之前没有检测过），就将其加入已检测方块数组
                    bDAdd = true;     //递归条件               
                }
                
            }
        }
        if (!bRAdd && !bLAdd && !bUAdd && !bDAdd) {
            return;
        }
        if (bRAdd)
            oneStarFourSide(col + 1, row);
        if (bLAdd)
            oneStarFourSide(col - 1, row);
        if (bUAdd)
            oneStarFourSide(col, row + 1);
        if (bDAdd)
            oneStarFourSide(col, row - 1);        
    }
    //初始化函数
    void Start()
    {
        //gParticle = GameObject.Find("Particle");        
        s_gOneStar = GameObject.Find("blue");
        s_gTwoStar = GameObject.Find("green");
        s_gThreeStar = GameObject.Find("orange");
        s_gFourStar = GameObject.Find("purple");
        s_gFiveStar = GameObject.Find("red");
        gOneStar = new GameObject[iCol, iRow];
        worldPoint = new Vector3[iCol, iRow];
        s_gStar = new GameObject[5] { s_gOneStar, s_gTwoStar, s_gThreeStar, s_gFourStar, s_gFiveStar };
        x = s_gOneStar.transform.position.x;
        y = s_gOneStar.transform.position.y;
        Vector3 position;
        //zeroPosition = s_gOneStar.transform.position;
        float fy = y;        
        y = y - 0.48f;      
        //从左往右从下到上建立方块
        for (int i = 0; i < iCol; i++)
        {
            for (int j = 0; j < iRow; j++)
            {
                y = y + 0.48f;
                position = new Vector3(x, y, -1);
                worldPoint[i,j] = position;
                int StarNumber = Random.Range(0, 5);
                gOneStar[i, j] = (GameObject)Instantiate(s_gStar[StarNumber], position, s_gOneStar.transform.rotation);
                gOneStar[i, j].transform.name = s_gStar[StarNumber].name;
            }
            x = x + 0.48f;
            y = fy - 0.48f;
        }
        //消除用于复制的方块
        GameObject.Destroy(s_gOneStar);
        GameObject.Destroy(s_gTwoStar);
        GameObject.Destroy(s_gThreeStar);
        GameObject.Destroy(s_gFourStar);
        GameObject.Destroy(s_gFiveStar);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Camera.main.WorldToScreenPoint(new Vector3(-2f, -3.5f, 0f)).x, Camera.main.WorldToScreenPoint(new Vector3(-2f, -3.5f, 0f)).y, 100, 100), "目标分数："+iToScore.ToString());
        GUI.Label(new Rect(Camera.main.WorldToScreenPoint(new Vector3(0.5f, -3.5f, 0f)).x, Camera.main.WorldToScreenPoint(new Vector3(0.5f, -3.5f, 0f)).y, 100, 100), "得分: "+iScore.ToString());        
    }
    //每帧更新
    void Update()
    {        
            if (Input.GetMouseButtonDown(0))
            {                
                starArray.Clear();
                //获取按键坐标
                float fWorldX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                float fWorldY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                float x = fWorldX + 2.4f;
                float y = fWorldY + 4.0f;
                int col = (int)(x / 0.48f);
                int row = (int)(y / 0.48f);
                if (col < iCol && row < iRow && x >= 0 && y >= 0 && gOneStar[col, row])
                {
                    oneStarFourSide(col, row);//寻找同色方块
                }

                if (starArray.Count >= iKillNum)//同色方块数不小于要消除的方块数
                {                    
                    iScore += 5 * starArray.Count * starArray.Count;                    
                    //重排序，X轴从小到大，Y轴从大到小
                    for (int i = 0; i < starArray.Count; i++)
                    {
                        Vector2 vColMin = (Vector2)starArray[i];
                        for (int j = i + 1; j < starArray.Count; j++)
                        {
                            if (((Vector2)starArray[j]).x < vColMin.x)
                            {
                                ArrayList vTemp = new ArrayList();
                                vTemp.Add(starArray[j]);
                                starArray[j] = starArray[i];
                                starArray[i] = vTemp[0];
                                vColMin = (Vector2)starArray[i];
                            }
                        }
                    }
                    for (int i = 0; i < starArray.Count; i++)
                    {
                        Vector2 vRowMax = (Vector2)starArray[i];
                        for (int j = i + 1; j < starArray.Count; j++)
                        {
                            if (((Vector2)starArray[j]).x == vRowMax.x && ((Vector2)starArray[j]).y > vRowMax.y)
                            {
                                ArrayList vTemp = new ArrayList();
                                vTemp.Add(starArray[j]);
                                starArray[j] = starArray[i];
                                starArray[i] = vTemp[0];
                                vRowMax = (Vector2)starArray[i];
                            }
                        }
                    }
                    //消除同色方块
                        for (int i = 0; i < starArray.Count; i++)
                        {
                            Vector2 v2Temp = (Vector2)starArray[i];                            
                            destroy((int)v2Temp.x, (int)v2Temp.y);
                            //particleFunc(col, row);
                        }
                    isEmptyCol();//检测是否有空列
                }
                isDead(iKillNum);
            }        
    }
}

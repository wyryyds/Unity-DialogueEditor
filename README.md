# Unity-DialogueEditor
## 适用于Unity项目的轻量对话编辑器
### 支持快速自定义多分支对话文本  
### 支持数据存储与加载  
对于它的使用您只需要提前在编辑器中设置好对话内容与分支对话的连接并保存数据，在需要加载对话的地方使用如下脚本进行加载  
```
DialogueManager.Instance.LoadDialogue(string containerName, TextMeshProUGUI textMeshProUGUI, List<Button> buttons);
```
其中 containerName 既是资源名也是资源在Resources文件夹下的路径  
textMeshProUGUI 是该节点的对话内容， buttons 是存储对话选项的按钮组件  

测试场景中使用的是TextMeshPro这一新文本组件，当然我也对旧的Text组件进行了兼容： 
```
DialogueManager.Instance.LoadDialogue(string containerName,Text mainText,List<Button> buttons)
```
bug略多，仍在维护中.....

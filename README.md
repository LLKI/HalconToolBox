# HalconToolBox
这是一个基于WPF和HALCON实现的工具箱，实现了MVTec HDevelop的部分功能，包括模板匹配、字符识别等功能

平台工具：VS,WPF,MVTec HDevelop XL 24.05 Progress,Prism

结构图：

![image](https://github.com/user-attachments/assets/e01d9f44-29df-4825-b40a-c320e15c1def)

# 主要算子
## 模板匹配
通过模板匹配算子在图像中查找与给定模板相似的区域
### 形状匹配
- create_shape_model：创建一个形状模型，用于后续的形状匹配。
- find_shape_model：在图像中查找与给定形状模型匹配的区域。
- GetShapeModelContours：获取形状模型的轮廓。
- VectorAngleToRigid：将旋转向量转换为刚体变换。
- AffineTransContourXld：对轮廓XLD应用仿射变换。
### 相似性匹配(灰度匹配)
- create_ncc_model：创建一个归一化互相关（NCC）模型，用于后续的灰度匹配。
- find_ncc_model：在图像中查找与给定NCC模型匹配的区域。
- Rgb1ToGray：将RGB图像转换为灰度图像。
- GenContourRegionXld：从轮廓生成XLD区域。
- AffineTransContourXld：对轮廓XLD应用仿射变换。
### 形变匹配
- create_local_deformable_model：创建一个局部可变形模型，用于后续的形变匹配。
- find_local_deformable_model：在图像中查找与给定局部可变形模型匹配的区域。
## 比较测量
### 卡尺找圆
- add_metrology_object_circle_measure：添加圆形测量对象。
- GetMetrologyObjectMeasures：获取测量对象的测量值。
- ApplyMetrologyModel：应用测量模型。
- GetMetrologyObjectResult：获取测量对象的结果。
### 颜色检测
- trans_from_rgb：从RGB颜色空间转换到其他颜色空间。
- decompose3：分解颜色空间。
### 几何测量
- sobel_amp：计算Sobel算子的幅值。
- CloseEdges：闭合边缘。
## 字符识别
### 简单字符识别
- create_text_model_reader：创建文本模型读取器。
- find_text：在图像中查找文本。
- GetTextObject：获取文本对象。
### 一维码识别
- create_bar_code_model：创建条码模型。
- find_bar_code：在图像中查找条码。
- GetBarCodeResult：获取条码识别结果。
### 二维码识别
- create_data_code_2d_model：创建二维数据码模型。
- find_data_code_2d：在图像中查找二维数据码。
## 缺陷检测
### 差分模型(定位+差分)
- FindShapeModel：在图像中查找与给定形状模型匹配的区域。
- train_variation_model：训练差分模型。
- prepare_variation_model：准备差分模型。
- GetVariationModel：获取差分模型。

# 部分效果展示
- 主页面

![image](https://github.com/user-attachments/assets/0835e7b5-33d9-4d50-8b36-cc7f561511a1)

- 形状匹配

![ShapeMatch](https://github.com/user-attachments/assets/518a4434-76ea-47e6-9220-3f8fad9abb11)

- 卡尺找圆

![找圆Sample](https://github.com/user-attachments/assets/61cf4782-3075-44ce-bd23-8464af0033c5)

- 二维码识别

![QRCode_识别](https://github.com/user-attachments/assets/5c2bb93d-5053-4329-bb87-0749c1c31768)

- 缺陷检测

![缺陷Sample](https://github.com/user-attachments/assets/f141aa22-b75d-47a4-96d7-b53e604d144f)

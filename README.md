# HalconToolBox
这是一个基于WPF和HALCON实现的工具箱，实现了MVTec HDevelop的部分功能，包括形状匹配、

平台工具：VS,WPF,MVTec HDevelop XL 24.05 Progress,Prism

结构图：

![image](https://github.com/user-attachments/assets/e01d9f44-29df-4825-b40a-c320e15c1def)

# 主要算子
## 模板匹配
### 形状匹配
- create_shape_model
- find_shape_model
- GetShapeModelContours
- VectorAngleToRigid
- AffineTransContourXld
### 相似性匹配(灰度匹配)
- create_ncc_model
- find_ncc_model
- Rgb1ToGray
- GenContourRegionXld
- AffineTransContourXld
### 形变匹配
- create_local_deformable_model
- find_local_deformable_model
## 比较测量
### 卡尺找圆
- add_metrology_object_circle_measure
- GetMetrologyObjectMeasures
- ApplyMetrologyModel
- GetMetrologyObjectResult
### 颜色检测
- trans_from_rgb
- decompose3
### 几何测量
- sobel_amp
- CloseEdges
## 字符识别
### 简单字符识别
- create_text_model_reader
- find_text
- GetTextObject
### 一维码识别
- create_bar_code_model
- find_bar_code
- GetBarCodeResult
### 二维码识别
- create_data_code_2d_model
- find_data_code_2d
## 缺陷检测
### 差分模型(定位+差分)
- FindShapeModel
- train_variation_model
- GetVariationModel
- prepare_variation_model
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

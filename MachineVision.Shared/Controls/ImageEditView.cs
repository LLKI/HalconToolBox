﻿using HalconDotNet;
using MachineVision.Core.TemplateMatch;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MachineVision.Shared.Controls
{
    public class ImageEditView:Control
    {
        private HSmartWindowControlWPF hSmart;
        private HWindow hWindow;
        public TextBlock txtMeg;



        public ObservableCollection<TemplateMatchResult> MatchResults
        {
            get { return (ObservableCollection<TemplateMatchResult>)GetValue(MatchResultsProperty); }
            set { SetValue(MatchResultsProperty, value); }
        }

        public static readonly DependencyProperty MatchResultsProperty =
            DependencyProperty.Register("MatchResults", typeof(ObservableCollection<TemplateMatchResult>), typeof(ImageEditView), new PropertyMetadata(MatchResultsCallBack));

        private static void MatchResultsCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEditView view && e.NewValue != null)
            {
                view.DisplayMatchRender();
            }
        }

        private void DisplayMatchRender()
        {
            foreach (var item in MatchResults)
            {
                hWindow.DispCross(item.Row, item.Column, 25, item.Angle);
            }
        }

        public HObject Image
        {
            get { return (HObject)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(HObject), typeof(ImageEditView), new PropertyMetadata(ImageChangedCallback));



        public ObservableCollection<DrawingObjectInfo> DrawObjectList
        {
            get { return (ObservableCollection<DrawingObjectInfo>)GetValue(DrawObjectListProperty); }
            set { SetValue(DrawObjectListProperty, value); }
        }

        public static readonly DependencyProperty DrawObjectListProperty =
            DependencyProperty.Register("DrawObjectList", typeof(ObservableCollection<DrawingObjectInfo>), typeof(ImageEditView), new PropertyMetadata(new ObservableCollection<DrawingObjectInfo>()));



        public static void ImageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEditView view && e.NewValue != null)
            {
                view.Display((HObject)e.NewValue);
            }
        }

        public void Display(HObject hObject)
        {
            hWindow.DispObj(hObject);
            hWindow.SetPart(0, 0, -2, -2);
        }

        public override void OnApplyTemplate()
        {
            txtMeg = (TextBlock)GetTemplateChild("PART_MEG");
            if (GetTemplateChild("PART_SMART") is HSmartWindowControlWPF hSmart)
            {
                this.hSmart = hSmart;
                this.hSmart.Loaded += HSmart_Loaded;
            }

            
            if (GetTemplateChild("PART_RECT") is Button btnRect)
            {
                btnRect.Click += (s, e) =>
                {
                    DrawRect();
                };
            }

            if (GetTemplateChild("PART_Ellipse") is Button btnEllipse)
            {
                btnEllipse.Click += (s, e) =>
                {
                    DrawEllipse();
                };
            }

            if (GetTemplateChild("PART_Circle") is Button btnCircle)
            {
                btnCircle.Click += (s, e) =>
                {
                    DrawCircle();
                };
            }

            if (GetTemplateChild("PART_Region") is Button btnRegion)
            {
                btnRegion.Click += (s, e) =>
                {
                    DrawRegion();
                };
            }

            if (GetTemplateChild("PART_ClearAll") is Button btnClear)
            {
                btnClear.Click += (s, e) =>
                {
                    ClearAll();
                };
            }

            base.OnApplyTemplate();
        }

        private async void ClearAll()
        {
            DrawObjectList?.Clear();
            hWindow.ClearWindow();
            if(Image!=null)
            {
                hWindow.DispObj(Image);
            }
        }

        private async void DrawCircle()
        {
            HTuple row = new HTuple();
            HTuple column = new HTuple();
            HTuple radius = new HTuple();
            HObject? CircleObj = null;

            txtMeg.Text = "按鼠标左键绘制，右键结束";
            await Task.Run(() =>
            {
                HOperatorSet.SetColor(hWindow, "blue");
                HOperatorSet.DrawCircle(hWindow, out row, out column, out radius);
                HOperatorSet.GenCircle(out CircleObj,row, column, radius);
            });
            if (CircleObj == null) { return; }
            DrawObjectList.Add(new DrawingObjectInfo()
            {
                ShapeType = ShapeType.Circle,
                HObject = CircleObj,
                HTuples = new HTuple[] { row, column,radius }
            });
            txtMeg.Text = String.Empty;
            HOperatorSet.DispObj(CircleObj,hWindow);
        }

        private async void DrawEllipse()
        {
            HTuple row = new HTuple();
            HTuple column = new HTuple();
            HTuple radius1 = new HTuple();
            HTuple radius2 = new HTuple();
            HTuple phi = new HTuple();
            HObject? EllispeObj = null;

            txtMeg.Text = "按鼠标左键绘制，右键结束";
            await Task.Run(() =>
            {
                HOperatorSet.SetColor(hWindow, "blue");
                HOperatorSet.DrawEllipse(hWindow, out row, out column, out phi,out radius1,out radius2);

                HOperatorSet.GenEllipse(out EllispeObj, row, column, phi,radius1,radius2);
            });
            if(EllispeObj == null) { return; }
            DrawObjectList.Add(new DrawingObjectInfo()
            {
                ShapeType = ShapeType.Ellipse,
                HObject= EllispeObj,
                HTuples = new HTuple[] { row, column, phi, radius1, radius2 }
            });
            txtMeg.Text = String.Empty;
            HOperatorSet.DispObj(EllispeObj, hWindow);
        }

        private async void DrawRect()
        {
            HTuple row1 = new HTuple();
            HTuple row2 = new HTuple();
            HTuple column1 = new HTuple();
            HTuple column2 = new HTuple();
            HObject? rectobj = null;
            txtMeg.Text = "按鼠标左键绘制，右键结束";
            await Task.Run(() =>
            {
                HOperatorSet.SetColor(hWindow, "blue");
                HOperatorSet.DrawRectangle1(hWindow, out row1, out column1, out row2, out column2);

                HOperatorSet.GenRectangle1(out rectobj, row1, column1, row2, column2);
            });

            if(rectobj == null) { return; }
            DrawObjectList.Add(new DrawingObjectInfo()
            {
                ShapeType = ShapeType.Rectangle,
                HObject = rectobj,
                HTuples = new HTuple[] { row1, column1, row2, column2 }
            }); 
            txtMeg.Text = String.Empty;
            HOperatorSet.DispObj(rectobj, hWindow);
        }

        private async void DrawRegion()
        {
            HObject? RegionObject = null;

            txtMeg.Text = "按鼠标左键绘制，右键结束";
            await Task.Run(() =>
            {
                HOperatorSet.SetColor(hWindow, "blue");
                HOperatorSet.DrawRegion(out RegionObject,hWindow);
            });
            if (RegionObject == null) { return; }
            DrawObjectList.Add(new DrawingObjectInfo()
            {
                ShapeType = ShapeType.Region,
                HObject = RegionObject
            });
            txtMeg.Text = String.Empty;
            HOperatorSet.DispObj(RegionObject, hWindow);
        }

        private void HSmart_Loaded(object sender, RoutedEventArgs e)
        {
            hWindow = this.hSmart.HalconWindow;
        }
    }
}
   M S B A M L   `   `   `  ����A  =kmeans, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:kmeanskmeans  5   	       kmeans.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:kmeans     5   	     � Title$  
MainWindow��5   	   $	��480��6   $��900         ��6)   .��5
      �  � ColumnDefinitions 5   
   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $��2* �6    � RowDefinitions 5   
   �� 5      $��* �6   6)   �� 5      $��* �6   6)   .+�5   
   �� -   5        $	��img��5   -   $��0��6   $��0��6    $��0,0,0,0q�6W   ��68   �� *SourceImage+6W   �� 5   
   -   5        $��img_Clustered��5   -   $��1��6   $��0��6    $��0,0,0,0q�6c   ��6B   �� *ClusterImage+6c   �� 5   
   -   5        $��result��5   -   $��2��6   $��0��6    $
��Left=�6]   $��0,0,0,0q�6y   ��6;   �� *SegmentedImage+6y   �� 5   
   -   5        $��Start��5   .   $��1��6   $��1��6!   $��Start��6;   $��0,168,103,33q�6K    J� RenderTransformOrigin$ 0.494,1.362&�6{   $��Right=�6�   $��84��6�   �� 5   
   -   5        $��0��5      $��1��6!   $��Kép kiválasztása��6.   $
��Left=�6I   $��23��6d   $��104,168,0,0q�6p   $	��Top=�6�   $	��104��6�   6�   �� 5   
   $��Clusterek száma:��6   $��29��6+   $��57,51,142,0q�67   $	��Top=�6L   $��1��6d   $��1��6t   �� 5   
   -   5        $��24��5      $��161,51,85,0q�6    �� TextWrapping$
 Wrap=�63   $	��Top=�6Z   $��1��6r   $��1��6�   ��6G   �� *K+6�   	� 5   
    	� 	GroupName$ feldold��6   $��Szekvenciálisan��6*   $��97,99,101,0q�6h   $	��Top=�6}   $��1��6�   $��1��6�    d� 	IsChecked 6D   �� *Szekvencialis+6�   	� 5   
   $ feldold��6   $��Párhuzamosan��6*   $��99,119,103,0q�6A   $	��Top=�6W   $��1��6o   $��1��6   �� 5    
    �� ContentStringFormat$) #Szekvenciális futási idő {0} sec��6?   $
��Left=�6v   $��	63,79,0,0q�6�   $	��Top=�6�   $��2��6�   $��1��6�   ��6   �� *TotalsecondsSzekvencialis+6�   �� 5!   
   $&  Párhuzamos futási idő {0} sec��6<   $
��Left=�6p   $��
63,111,0,0q�6�   $	��Top=�6�   $��2��6�   $��1��6�   ��6   �� *TotalsecondsParhuzamos+6�   �� 5"   
   $��Eredeti kép��6   $��114,51,0,147q�6&   $��1��6=   $
��Left=�6J   $��69��6e   �� 5#   
   $��Kapott kép��6   $��27��6%   $��
99,25,98,0q�61   $	��Top=�6E   $��2��6]   $��1��6m   5&      5'      
   M S B A M L   `   `   `  ����J  FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner  5   	   C&clr-namespace:DiagramDesigner.ControlsDiagramDesigner.Controls  5   	       DiagramDesigner.Window1   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   +sclr-namespace:DiagramDesigner     5   	   4c&clr-namespace:DiagramDesigner.Controls     5   	     � WindowStartupLocation$  CenterScreen=�5   	    J� SnapsToDevicePixels .  � Title$ Diagram Designer��5   	    � WindowState$ 	Maximized=�5	   	    � 	Resources 5         DesignerCanvasContextMenu$   DiagramDesigner.DesignerCanvas   	SelectAll m� Command��%T   &	        �� 5   
   .@�5      m� $��
Select All��6   # Z 6+   5      5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.     ToolbarGroupHeaderBrush#���  6;   .+�5      ��   	MyToolbar#��[ 6   �� 5      $��SearchResult��6   
���   �  ToolbarGroup#��[ 6@   $��	450,3,3,3q�6f   .��5      #   DiagramDesigner.SearchTextBox  -   5        	  Name$	 SearchFrame��5   d   $��40��6"   $	��350��6.   $��5,30,5,0q�6:   $	��Top=�6L   
�������6�   5'      5(      � 5+   
   $��1��6   $��0,10,0,0q�6   
 � ColumnDefinitions
 5,      �� 5-      $	��275 �6#   �� 5.      .+�50      �� -   5         ��Name$ SiderbarScrollViewer��50      
������ $
��Auto=�52   #   .��54      � $��0��6   $��0,0,5,0q�6,   
�������.+�55      �� -   5        $  SymbolStencilsScrollViewer��55   #   
������ $
��Auto=�57   #   .��58      9� $��Trigger��6#   $	��250��64   $	��255��6A     SymbolStencils#��[ 6M    9� 
IsExpanded . 6w   59      �� 5:      -   5        $# ConditionStencilsScrollViewer��5:   #   
������ $
��Auto=�5<   #   .��5=      9� $��	Condition��6#   $	��250��66   $	��255��6C     ConditionStencils#��[ 6O    . 6|   5>      �� 5?      -   5        $  ActionStencilsScrollViewer��5?   #   
������ $
��Auto=�5A   #   .��5B      9� $��Action��6#   $	��250��63   $	��255��6@     ActionStencils#��[ 6L    . 6v   5C      �� 5D      -   5        $  TargetStencilsScrollViewer��5D   #   
������ $
��Auto=�5F   #   .��5G      9� $��Target��6#   $	��250��63   $	��255��6@     TargetStencils#��[ 6L    . 6v   5H      5J      5K      �� 5N      |�.  $��2��6-   
�������$��Stretch=�5O      $��Right=�61   �� 5Q      $��3,1,0,0q�6   $��1��5R      
�������$��1q�5T      ._�5U      �� $��Diagram��6   $��1��6,   $��3,0,0,0q�6<   ��6M   Z� .��5V      �  5W      �� 5X      $��2* �6,   �� 5Y      $	��11* �6,   �� 5Z      $��2* �6,   .+�5\      �� -   5        $ TargetScrollViewer��5\   '   
������ $��0��5^   #   $
��Auto=�5_   #   $
��Auto=�5`   #   .��5a      �� -   5         ��Name$ TargetPanel��5a   )   
�������$ 
Horizontal=�5c   )   $��50��5d   )   5e      5f      �� 5h      -	   5        $ DesignerScrollViewer��5h   '   
������ $��1��5j   #   $
��Auto=�5k   #   $
��Auto=�5l   #   .��5m        -
   5          Name$ MyDesignerCanvas��5m   /   |�. $	��800��5o   +   $
��1600��5p   +    J� 	AllowDrop .   WindowBackgroundBrush#��[ 5r   +   $��10q�5s   +    � ContextMenu# [  5t   +    � FocusVisualStyle 5s   7   Z� 5u      5v      �� 5x      -   5        $ AiBarScrollViewer��5x   '   
������ $��2��5z   #   $
��Auto=�5{   #   $
��Auto=�5|   #   .��5}      �� -   5        $ AiBar��5}   J   $
��1600��6)   $��0,0,0,0q�69   $ 
Horizontal=�6Y   5~      5         DiagramDesigner.ZoomBox  5�      $	��280��6$   $��Right=�5�       $	��Top=�5�       $��1��5�   Z   $��0,0,22,0q�6g      ScrollViewer 6    ��  �� ElementName$ DesignerScrollViewer��6g   5�      5�      5�      5�      5�      5�      
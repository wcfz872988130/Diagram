   M S B A M L   `   `   `  ���� J  FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner  C&clr-namespace:DiagramDesigner.ControlsDiagramDesigner.Controls      DiagramDesigner.Window1   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     +sclr-namespace:DiagramDesigner     4c&clr-namespace:DiagramDesigner.Controls       � WindowStartupLocation$  CenterScreen=� J� SnapsToDevicePixels .  � Title$ Diagram Designer�� � WindowState$ 	Maximized=� � 	Resources    DesignerCanvasContextMenu$   DiagramDesigner.DesignerCanvas   	SelectAll m� Command��%/   &	        �� .@�m� $��
Select All��# Z .��� $��10q� � RowDefinitions �� $
��Auto ��� $��* �.+���  �� Orientation$ 
Horizontal=�$��0��  ToolbarGroupHeaderBrush#���  .+���   	MyToolbar#��[ �� $��SearchResult��
���   �  ToolbarGroup#��[ $��	450,3,3,3q�.��#   DiagramDesigner.SearchTextBox  -   	  Name$	 SearchFrame��$��40��$	��350��$��5,30,5,0q�$	��Top=�
�������� $��1��$��0,10,0,0q�
 � ColumnDefinitions
 �� $	��275 ��� .+��� -    ��Name$ SiderbarScrollViewer��
������ $
��Auto=�.��� $��0��$��0,0,5,0q�
�������.+��� -   $  SymbolStencilsScrollViewer��
������ $
��Auto=�.��9� $��Trigger��$	��250��$	��255��  SymbolStencils#��[  9� 
IsExpanded . �� -   $# ConditionStencilsScrollViewer��
������ $
��Auto=�.��9� $��	Condition��$	��250��$	��255��  ConditionStencils#��[  . �� -   $  ActionStencilsScrollViewer��
������ $
��Auto=�.��9� $��Action��$	��250��$	��255��  ActionStencils#��[  . �� -   $  TargetStencilsScrollViewer��
������ $
��Auto=�.��9� $��Target��$	��250��$	��255��  TargetStencils#��[  . �� |�.  $��2��
�������$��Stretch=�$��Right=��� $��3,1,0,0q�$��1��
�������$��1q�._��� $��Diagram��$��1��$��3,0,0,0q���Z� .���  �� $��2* ��� $	��11* ��� $��2* �.+��� -   $ TargetScrollViewer��
������ $��0��$
��Auto=�$
��Auto=�.���� -    ��Name$ TargetPanel��
�������$ 
Horizontal=�$��50���� -	   $ DesignerScrollViewer��
������ $��1��$
��Auto=�$
��Auto=�.��  -
     Name$ MyDesignerCanvas��|�. $	��800��$
��1600�� J� 	AllowDrop .   WindowBackgroundBrush#��[ $��10q� � ContextMenu# [   � FocusVisualStyle Z� �� -   $ AiBarScrollViewer��
������ $��2��$
��Auto=�$
��Auto=�.���� -   $ AiBar��$
��1600��$��0,0,0,0q�$ 
Horizontal=�   DiagramDesigner.ZoomBox  $	��280��$��Right=�$	��Top=�$��1��$��0,0,22,0q�   ScrollViewer ��  �� ElementName$ DesignerScrollViewer��
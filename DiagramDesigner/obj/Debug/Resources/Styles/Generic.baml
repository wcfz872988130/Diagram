   M S B A M L   `   `   `  ����J  FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   SearchTextBox_Background  �� Color  SearchTextBox_Foreground  SearchTextBox_Border �� 
StartPoint �� EndPoint � Color � Offset ! SearchTextBox_BorderMouseOver " SearchTextBox_SearchIconBorder & "SearchTextBox_SearchIconBackground , (SearchTextBox_SearchIconBorder_MouseOver 0 ,SearchTextBox_SearchIconBackground_MouseOver , (SearchTextBox_SearchIconBorder_MouseDown 0	 ,SearchTextBox_SearchIconBackground_MouseDown  
 SearchTextBox_LabelTextColor#    DiagramDesigner.SearchTextBox �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �Name � ColumnDefinitions ��Name ��Name �� RelativeSource �� Path ��Name � ToolTip �� Triggers P� 
SourceName P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      #lclr-namespace:DiagramDesigner  5      %�  &	        &	      &	 >     &	     &	 �    &	 �    &	     &	 �    &	 �    &		 E    &	
 B    '   `    7   7  7  7 
 7  7  7  7  �� 5      $  White��67   �� 5      $  Black��67   �� 5      $	 0,0&�67   $	 0,1&�6H   .��5   
   � $ 	#FFABADB3��6   $
 0.05Q�6)   � 5   
   $ 	#FFE2E3EA��6   $
 0.07Q�6)   � 5	   
   $ 	#FFE4E9EF��6   $ 1Q�6)   5
      �� 5      $	 0,0&�6@   $	 0,1&�6Q   .��5   
   � $ 	#FF5C97C1��6   $
 0.05Q�6)   � 5   
   $ 	#FFB9D7EB��6   $
 0.07Q�6)   � 5   
   $ 	#FFC7E2F1��6   $ 1Q�6)   5      �� 5      $  White��6=   �� 5      $  White��6A   �� 5      $	 0,0&�6K   $	 0,1&�6\   .��5   
   � $ 	#FFFFFFFF��6   $ 0Q�6)   � 5   
   $ 	#FFE5F4FC��6   $ 1Q�6)   5      �� 5      $	 0,0&�6O   $	 0,1&�6`   .��5   
   � $ 	#FFE7F5FD��6   $ 0Q�6)   � 5   
   $ 	#FFD2EDFC��6   $	 0.5Q�6)   � 5   
   $ 	#FFB6E3FD��6   $
 0.51Q�6)   � 5   
   $ 	#FF9DD5F3��6   $ 1Q�6)   5      �� 5      $	 0,0&�6K   $	 0,1&�6\   .��5   
   � $ 	#FFFFFFFF��6   $ 0Q�6)   � 5   
   $ 	#FFE5F4FC��6   $ 1Q�6)   5      �� 5       $	 0,0&�6O   $	 0,1&�6`   .��5!   
   � $ 	#FFE7F5FD��6   $ 0Q�6)   � 5"   
   $ 	#FFD2EDFC��6   $	 0.5Q�6)   � 5#   
   $ 	#FFB6E3FD��6   $
 0.51Q�6)   � 5$   
   $ 	#FF9DD5F3��6   $ 1Q�6)   5%      �� 5&      $
  Gray��6;   �� 5(      "   6-   .�5)   
   ��  � ��8   6'   �� 5*   
    � ��8  6(   �� 5+   
    � ��8  6'   �� 5,   
    � ��$ 1q�6,   �� 5-   
    �@J�SnapsToDevicePixels . 60   �� 5.   
    �@  	LabelText$ Search��6&   �� 5/   
    �@�FocusVisualStyle 6-   Z� �� 50   
    �@  LabelTextColor8  6+   �� 51   
    � �� 52      �� 53      "   6"   .R�54      �� $	 Border��6   #��z��55      #��z��56      #��z��57      ._�58      � $
 
LayoutGrid��6    59      �� 5:   "   $��* �63   �� 5;   "   $
��Auto �63   �� 5<   "   $
��Auto �63   �� 5=   "   $
��Auto �63   .+�5?      �� $ PART_ContentHost��66   $��2q�6+   $��0��6P   $��Center=�6`   �� 5@      $ 	LabelText��6$   $��2q�5A      $��0��5B      $��2,0,0,0q�5G      $��Center=�5H      $��Italic$�5I      ��5C      ��  �� *TemplatedParent+$ LabelTextColor���5E      ��  �� *TemplatedParent+$ 	LabelText�5I      �� 5J      $	 PART_SearchIconBorder��6%   $��2��5K   %   $��Stretch=�5L   %   $��Stretch=�5M   %   8�� ._�5O   "   �� $ 
SearchIcon��6(   $
��None=�5P   (   $��15��5Q   (   $��15��5R   (   $��Center=�5S   (   $��Center=�5T   (   $R��Lpack://application:,,,/DiagramDesigner;component/Resources/Images/search.png��5U   (   $ Click to search��6~   5V      �� 5X      $	 PART_SpecifySearchType��6%   $��1��5Y   '   $��Stretch=�5Z   '   $��Stretch=�5[   '   8�� ._�5]   "   �� $ SpecifySearchType��6(   $
��None=�5^       $��15��5_       $��15��5`       $��Center=�5a       $��Center=�5b       $S��Mpack://application:,,,/DiagramDesigner;component/Resources/Images/Key-Add.png��5c       $ Choose section(s)��6w   5f      �� 5h      $	 PART_PreviousItem��6%   $��3��5i      $��Stretch=�5j      $��Stretch=�5k      8�� ._�5m   "   �� $ PreviousIcon��6(   $
��None=�5n      $��15��5o      $��15��5p      $��Center=�5q      $��Center=�5r      $T��Npack://application:,,,/DiagramDesigner;component/Resources/Images/Previous.png��5s      $ Previous keyword(s)��6t   5u      5v      5w       5x      P� 5y      $ 	LabelText��6F    �@J�IsMouseOver . . �5z      �� $ 
LayoutGrid��6;    � �� �#F3F9F26S   �� 5{       �@�Cursor$ IBeam��67   5|      P� 5~       �@J�IsMouseOver .  . �5      �� $ 
LayoutGrid��6;    � �� �#DCEED96S   5�      P� 5�       �@J�IsKeyboardFocusWithin . . �5�      �� 8   � ��6<   5�      P� 5�       �@  HasText . . �5�      �� $ 	LabelText��6;    � y�$ Hidden=�6R   5�      ]� 5�       5�      �� 5�   "    �@  HasText . 6?   ./�5�      �� $ 
SearchIcon��5�       � ��$Q Kpack://application:,,,/DiagramDesigner;component/Resources/Images/clear.png��5�      5�      5�      5�      5�      5�      
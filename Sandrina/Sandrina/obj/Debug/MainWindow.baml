   M S B A M L   `   `   `  ����C  ?Sandrina, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:SandrinaSandrina  5      =#clr-namespace:Sandrina.UserElementsSandrina.UserElements  5          Sandrina.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      (localclr-namespace:Sandrina     5      6custom#clr-namespace:Sandrina.UserElements     5        � Title$  
MainWindow��5	       � WindowStyle$
 None=�5
       � 
ResizeMode$ CanResizeWithGrip=�5       � AllowsTransparency . $	��550��5      $	��800��5       � 	Resources 5         BarButtonForeground �� Color  TopBarButtonStyle �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name �� CornerRadius J� SnapsToDevicePixels ��Name �� Triggers P� Property P� Value �� 
TargetName��%s  &	        &	 %     �� 5   
   $ #DDDDDD��5      �� 5   
   " ��6*   .�5      ��  � ��$ 24��6&   �� 5       � ��$ 24��6'   �� 5       � ��$ Center=�62   �� 5       � ��$ Right=�64   �� 5       � ��$ 3q�6'   �� 5       � ��# �   6+   �� 5       � ��
 ���� 6+   �� 5       � ��$ 0q�60   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $
 border��6!   $ 6��5    	   #��z��5!   	   #��z��5"   	   #��z��5#   	    . ._�5%      �� $ contentPresenter��6/   |�.  #��z��5'      #��z��5(      ��. # z 5*      #��z��5+      5,       5-      P� 5.       �@J�IsMouseOver . . �50   "   �� $ border��6)    � �� �	#55dddddd52      �� 53   "    �@�Cursor$
 Hand��54      55      57      59      5:      5;      .��5=      �  � RowDefinitions 5>   
   �� 5?      $��30 �6   �� 5@      $��1* �6   .+�5C   
   � -   5         �Name$ TopBar��5C      ���#3E5D7A � ColumnDefinitions 5F      �� 5G      $��1* �6#   �� 5H      $��30 �6#   �� 5I      $��30 �6#   �� 5J      $��30 �6#   .+�5M      �� -   5         ��Name$ 
HideButton��5M      $	��➖��5N      #���  5O      $��1��5Q      �� 5R      -   5        $ ResizeButton��5R      $��O��5S      $��16(�5T      $��DemiBold"�5U      #���  5V      $��2��5X      �� 5Y      -   5        $ 
ExitButton��5Y      $	��✖��5Z      #���  5[      $��3��5]      5^      � 5`   
   -   5        $ MainGrid��5`      $��1��5a      .+�5c      '   !Sandrina.UserElements.HelloScreen  -   5          Name$ HelloScreenPage��5c   !   $y�Visible=�5d   	   (   "Sandrina.UserElements.Instructions  5f      -   5          Name$ InstructionsPage��5f   "   $y�Hidden=�5g   
       Sandrina.UserElements.Game  5i      -   5          Name$ GamePage��5i      $y�Hidden=�5j      5k      5l      5m      
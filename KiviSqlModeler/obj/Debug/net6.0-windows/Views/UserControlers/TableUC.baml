   M S B A M L   `   `   `  ����I  EKiviSqlModeler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:KiviSqlModeler.Views.UserControlers#KiviSqlModeler.Views.UserControlers  5      1    +KiviSqlModeler.Views.UserControlers.TableUC   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      A =Wpf.Ui, Version=2.0.3.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5      =local1clr-namespace:KiviSqlModeler.Views.UserControlers  5      .��5
      �   � RowDefinitions  5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   .+�5   
   �� $��0��6   $
��Left=�5      ��5      �� *	NameTable+&   System.Windows.Controls.DataGrid  5   
   -   5          Name$ dgTable��5      $��1��6$      HeadersVisibility$
 None=�5         AutoGenerateColumns$ False��5         CanUserAddRows$ False��5         CanUserDeleteRows$ False��5         
IsReadOnly$
 True��5      ��5      �� *	Columns+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      -   5        Key��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 auto 5   %   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 5   %   �� *
PkfkString+5   %     5      -   5        Name��$
 auto 5   %   	 5   %   �� *Name+5   %     5       -   5        Type��$
 auto 5"   %   	 5!   %   �� *Type+5"   %     5#      -   5        Null��$
 auto 5%   %   	 5$   %   �� *IsNull+5%   %   5'      5(      5)      
   M S B A M L   `   `   `  ????I  EKiviSqlModeler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:KiviSqlModeler.MyUserControlKiviSqlModeler.MyUserControl  5      (    "KiviSqlModeler.MyUserControl.Table   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:KiviSqlModeler.MyUserControl     5      .??5	      ]? .+?5
   
   ?? -   5        $??	tableName??5
      $	??Top=?6%     ?? TextAlignment$
  Left=?6:   ??6O   ?? *	NameTable+? 5   
   .+?5      ?? $??1q?6   
???   ?._?5      ? $??3??6   $??1??6'    ? RowDefinitions 5      ?? 5      $??* ?6(   ?? 5      $??* ?6(    ? ColumnDefinitions 5      ?? 5      $??3* ?6+   ?? 5      $??3* ?6+   ?? 5      $??1* ?6+   .+?5      ?? -   5         ??Name$ tbPrimaryKey??5      
?????? ??64   ?? *	Pk.name+6M   ?? 5      -   5        $ tBType??5      $??1??6.   
?????? ??6>   ?? *	Pk.type+6W   ?? 5      -   5        $ tpKey??5      $??2??6-   
?????? ??6=   ?? *	Pk.pkfk+6V   ?? 5      $??3, 20q?6    
???   ?$??1??6B   $??3??6M   $??2??6a   ?? 5      -   5         ??Name$ tbColumn??5      $??1??61   
?????? ??5      ?? 5       .R?5!   "   ?? ??6,   ?? *name+5"      5$      ?? 5%      -   5        $ tBType2??5%      $??1??60   $??1??6=   
?????? ??5&      ?? 5'      .R?5(   "   ?? ??6,   ?? *type+5)      5+      ?? 5,      -   5        $ tpKey2??5,      $??1??6/   $??2??6<   
?????? ??5-      ?? 5.      .R?5/   "   ?? ??6,   ?? *pkfk+50      52      53      54      55      56      57      
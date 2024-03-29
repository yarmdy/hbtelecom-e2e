USE [CTCCGoods]
GO
SET IDENTITY_INSERT [dbo].[cclass] ON 

GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (1, N'', N'室内-BBU')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (2, N'', N'室内-RRU')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (3, N'', N'室内-板卡')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (4, N'', N'室内-PB')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (5, N'', N'室内-Prru')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (6, N'', N'室内-交转直模块')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (7, N'', N'室外-BBU')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (8, N'', N'室外-RRU')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (9, N'', N'室外-板卡')
GO
INSERT [dbo].[cclass] ([id], [code], [name]) VALUES (10, N'', N'室外-小区Lisence')
GO
SET IDENTITY_INSERT [dbo].[cclass] OFF
GO
SET IDENTITY_INSERT [dbo].[cdesignset] ON 

GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (1, 17, 1)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (2, 17, 6)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (3, 17, 8)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (4, 17, 4)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (5, 17, 7)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (6, 17, 5)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (7, 18, 2)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (8, 18, 9)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (9, 19, 11)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (10, 20, 10)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (11, 20, 12)
GO
INSERT [dbo].[cdesignset] ([id], [uid], [wid]) VALUES (12, 20, 3)
GO
SET IDENTITY_INSERT [dbo].[cdesignset] OFF
GO
SET IDENTITY_INSERT [dbo].[cgoods] ON 

GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (1, N'', N'华为室内-BBU', 1, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (2, N'', N'华为室内-RRU', 2, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (3, N'', N'华为室内-板卡', 3, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (4, N'1.0版本', N'PB3902', 4, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (5, N'1.0版本', N'Prru3902-内置天线', 5, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (6, N'1.0版本', N'Prru3902-外置天线', 5, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (7, N'2.0版本', N'PB3912', 4, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (8, N'2.0版本', N'Prru3912-内置天线', 5, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (9, N'2.0版本', N'Prru3912-外置天线', 5, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (10, N'3.0版本', N'PB3552', 4, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (11, N'3.0版本', N'Prru3552-内置天线', 5, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (12, N'3.0版本', N'Prru3552-外置天线', 5, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (13, N'', N'华为室内-交转直模块', 6, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (14, N'', N'华为室外-BBU', 7, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (15, N'', N'华为室外-RRU', 8, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (16, N'', N'华为室外-板卡', 9, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (17, N'', N'华为室外-小区Lisence', 10, 14)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (18, N'', N'中兴室内-BBU', 1, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (19, N'', N'中兴室内-RRU', 2, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (20, N'', N'中兴室内-板卡', 3, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (21, N'1.0版本', N'PB1000', 4, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (22, N'1.0版本', N'Prru8108-内置天线', 5, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (23, N'1.0版本', N'Prru8108-外置天线', 5, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (24, N'2.0版本', N'PB1120', 4, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (25, N'2.0版本', N'Prru8119-内置天线', 5, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (26, N'2.0版本', N'Prru8119-外置天线', 5, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (27, N'', N'中兴室内-交转直模块', 6, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (28, N'', N'中兴室外-BBU', 7, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (29, N'', N'中兴室外-RRU', 8, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (30, N'', N'中兴室外-板卡', 9, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (31, N'', N'中兴室外-小区Lisence', 10, 15)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (32, N'', N'诺基亚室内-BBU', 1, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (33, N'', N'诺基亚室内-RRU', 2, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (34, N'', N'诺基亚室内-板卡', 3, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (35, N'', N'诺基亚室内PB', 4, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (36, N'', N'诺基亚室内Prru', 5, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (37, N'', N'诺基亚室内-交转直模块', 6, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (38, N'', N'诺基亚室外-BBU', 7, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (39, N'', N'诺基亚室外-RRU', 8, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (40, N'', N'诺基亚室外-板卡', 9, 16)
GO
INSERT [dbo].[cgoods] ([id], [code], [name], [cid], [pid]) VALUES (41, N'', N'诺基亚室外-小区Lisence', 10, 16)
GO
SET IDENTITY_INSERT [dbo].[cgoods] OFF
GO
SET IDENTITY_INSERT [dbo].[cuser] ON 

GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (1, N'admin', N'省公司', 0, 1, N'Hbdx1331', -1, N'', N'徐建涛')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (2, N'baoding', N'保定', 1, 1, N'Hbdx1331', 3, N'18931217286', N'刘丛')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (3, N'cangzhou', N'沧州', 1, 1, N'Hbdx1331', 4, N'18932789299', N'齐宗江')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (4, N'chengde', N'承德', 1, 1, N'Hbdx1331', 5, N'18903147092', N'吴霜')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (5, N'handan', N'邯郸', 1, 1, N'Hbdx1331', 2, N'18931017176', N'王海涛')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (6, N'hengshui', N'衡水', 1, 1, N'Hbdx1331', 7, N'18903187801', N'赵策')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (7, N'langfang', N'廊坊', 1, 1, N'Hbdx1331', 6, N'18903267219', N'周国正')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (8, N'qinhuangdao', N'秦皇岛', 1, 1, N'Hbdx1331', 8, N'18903346460', N'裴炳鑫')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (9, N'shijiazhuang', N'石家庄', 1, 1, N'Hbdx1331', 1, N'18931177141', N'杨诚')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (10, N'tangshan', N'唐山', 1, 1, N'Hbdx1331', 11, N'18903153083', N'郭金鑫')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (11, N'xingtai', N'邢台', 1, 1, N'Hbdx1331', 10, N'18903197736', N'孙小川')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (12, N'xiongan', N'雄安', 1, 1, N'Hbdx1331', 12, N'18133847198', N'室分主管')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (13, N'zhangjiakou', N'张家口', 1, 1, N'Hbdx1331', 9, N'18903230075', N'赵清海')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (14, N'huawei', N'华为', 3, 1, N'Hbdx1331', -1, N'17363115973/18031262605', N'孟凡杰/卢阳')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (15, N'zte', N'中兴', 3, 1, N'Hbdx1331', -1, N'18603119975', N'高建涛')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (16, N'nokia', N'诺基亚', 3, 1, N'Hbdx1331', -1, N'18132040001', N'谢哲')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (17, N'SHyuan', N'上海院', 2, 1, N'Hbdx1331', -1, N'18903830396', N'张彦海')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (18, N'Hxyuan', N'华信院', 2, 1, N'Hbdx1331', -1, N'13383213116', N'王辉')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (19, N'JSyuan', N'江苏院', 2, 1, N'Hbdx1331', -1, N'18951998917', N'胡剑飞')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (20, N'ZTyuan', N'中通院', 2, 1, N'Hbdx1331', -1, N'17703322013', N'邢金柱')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (21, N'sjz_zte', N'石家庄中兴', 4, 1, N'Hbdx1331', -1, N'17633242008', N'王云飞')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (22, N'sjz_hw', N'石家庄华为', 4, 1, N'Hbdx1331', -1, N'13903111820', N'于海涛')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (23, N'bd_zte', N'保定中兴', 4, 1, N'Hbdx1331', -1, N'15933936622', N'杨双')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (24, N'bd_nokia', N'保定诺基亚', 4, 1, N'Hbdx1331', -1, N'18731989901', N'曹祥')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (25, N'ts_hw', N'唐山华为', 4, 1, N'Hbdx1331', -1, N'17731500688', N'杨鑫宇')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (26, N'ts_nokia', N'唐山诺基亚', 4, 1, N'Hbdx1331', -1, N'13231507670', N'赵佳男')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (27, N'lf_hw', N'廊坊华为', 4, 1, N'Hbdx1331', -1, N'15613793293', N'李晨亮')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (28, N'hd_hw', N'邯郸华为', 4, 1, N'Hbdx1331', -1, N'17320666607', N'马志彬')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (29, N'cz_hw', N'沧州华为', 4, 1, N'Hbdx1331', -1, N'13292785215', N'常进')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (30, N'xt_hw', N'邢台华为', 4, 1, N'Hbdx1331', -1, N'18631978905', N'刘予省')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (31, N'xt_nokia', N'邢台诺基亚', 4, 1, N'Hbdx1331', -1, N'15613963006', N'杨柳')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (32, N'zjk_zte', N'张家口中兴', 4, 1, N'Hbdx1331', -1, N'18631305600', N'于成博')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (33, N'zjk_nokia', N'张家口诺基亚', 4, 1, N'Hbdx1331', -1, N'15324236882', N'米付治')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (34, N'hs_hw', N'衡水华为', 4, 1, N'Hbdx1331', -1, N'13253293688', N'鲁吉祥')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (35, N'cd_zte', N'承德中兴', 4, 1, N'Hbdx1331', -1, N'18603143344', N'佟子旭')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (36, N'xz_zte', N'雄安中兴', 4, 1, N'Hbdx1331', -1, N'15032495413', N'候硕桃')
GO
INSERT [dbo].[cuser] ([id], [code], [name], [utype], [status], [pwd], [wid], [tel], [contacts]) VALUES (37, N'qhd_hw', N'秦皇岛华为', 4, 1, N'Hbdx1331', -1, N'18032297653
', N'李海超')
GO
SET IDENTITY_INSERT [dbo].[cuser] OFF
GO
SET IDENTITY_INSERT [dbo].[cwarehouse] ON 

GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (1, N'', N'石家庄')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (2, N'', N'邯郸')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (3, N'', N'保定')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (4, N'', N'沧州')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (5, N'', N'承德')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (6, N'', N'廊坊')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (7, N'', N'衡水')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (8, N'', N'秦皇岛')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (9, N'', N'张家口')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (10, N'', N'邢台')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (11, N'', N'唐山')
GO
INSERT [dbo].[cwarehouse] ([id], [code], [name]) VALUES (12, N'', N'雄安')
GO
SET IDENTITY_INSERT [dbo].[cwarehouse] OFF
GO

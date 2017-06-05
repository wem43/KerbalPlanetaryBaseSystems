## Translation Guide

First of all thank yor for looking at these files in consideration to help translate Kerbal Planetary Base System into your language!

### How to translate
To create a translation for your language, make a copy of the files *en_us.cfg* and *KSPedia_en.cfg* and name 
them accordingly to your language:
* *es-es.cfg* and *KSPedia_es-es.cfg* for spanish
* *es-mx.cfg* and *KSPedia_es-mx.cfg* for mexican spanish
* *ja.cfg* and *KSPedia_ja.cfg* for japanese
* *ru.cfg* and *KSPedia_ru.cfg* for russian
* *zh-cn.cfg* and *KSPedia_zh-cn.cfg* for simplified chinese

Then change the language tag in the third line of the copy of the *en_us.cfg* file to the tag of your language.

*Again:*
* "es-es" for spanish
* "es-mx" for mexican spanish
* "ja" for japanese
* "ru" for russian
* "zh-cn" for simplified chinese

Finally translate all the string after the "=" signs.
e.g. for russian from 
    
    #LOC_KPBS.filtersettings.name = Filter Settings
    
to

    #LOC_KPBS.filtersettings.name = Настройки фильтра

### What not to translate
There are some texts in here that should not be translated into another language and be kept in the files as is
1. any word that starts with cck-XXX, e.g. cck-lifesupport. These are tags to sort the parts into categories by the Community Category Kit.
2. control sequences like '\n', '\t' or similar.
3. HTML Tags like &lt;b&gt;...&lt;/b&gt;, &lt;i&gt;...&lt;/i&gt; or similar  

And once again, thank you for helping translate this mod!

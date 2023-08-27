using System.Collections.ObjectModel;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services.WikiServices;
using CrossWikiEditor.Utils;
using CrossWikiEditor.ViewModels.ControlViewModels;
using CrossWikiEditor.ViewModels.MenuViewModels;
using CrossWikiEditor.ViewModels.ReportViewModels;
using ReactiveUI.Fody.Helpers;

namespace CrossWikiEditor.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private static string HtmlContnet =
        """
        <tr>
          <td colspan="2" class="diff-lineno">Տող  85.</td>
          <td colspan="2" class="diff-lineno">Տող  85.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt;։</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt;։</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Տաբեյը յոթ գիրք է հեղինակել, շրջակա միջավայրի պաշտպանության շրջանակներում ծրագրեր է ձեռնարկել՝ լեռնագնացների Էվերեստի վրա թողած աղբը հավաքելու նպատակով, ու [[Ֆուձիյամա|Ֆուջի լեռան]] ամենամյա արշավանքներ է <del class="diffchange diffchange-inline">առաջնորդել՝</del> [[Երկրաշարժ Ճապոնիայում (2011)|2011 թվականի երկրաշարժից]] տուժած երեխաների ու երիտասարդների համար։</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Տաբեյը յոթ գիրք է հեղինակել, շրջակա միջավայրի պաշտպանության շրջանակներում ծրագրեր է ձեռնարկել՝ լեռնագնացների Էվերեստի վրա թողած աղբը հավաքելու նպատակով, ու [[Ֆուձիյամա|Ֆուջի լեռան]] ամենամյա արշավանքներ է <ins class="diffchange diffchange-inline">առաջնարոդել՝</ins> [[Երկրաշարժ Ճապոնիայում (2011)|2011 թվականի երկրաշարժից]] տուժած երեխաների ու երիտասարդների համար։</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>[[(6897) Տաբեյ]] [[աստերոիդ]]ը իր անունը ստացել է Ջունկո Տաբեյի պատվին։ [[2019]] թվականին [[Պլուտոն]]ի վրա գտնվող լեռնաշղթա նույնպես Ջունկո Տաբեյի պատվին անվանվել է {{lang-en|«Tabei Montes»}}։</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>[[(6897) Տաբեյ]] [[աստերոիդ]]ը իր անունը ստացել է Ջունկո Տաբեյի պատվին։ [[2019]] թվականին [[Պլուտոն]]ի վրա գտնվող լեռնաշղթա նույնպես Ջունկո Տաբեյի պատվին անվանվել է {{lang-en|«Tabei Montes»}}։</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  95.</td>
          <td colspan="2" class="diff-lineno">Տող  95.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>Ջունկո Իշիբաշին ծնվել է [[1939 թվական]]ի [[սեպտեմբերի 22]]-ին [[Միհարու]]յում՝ [[Ֆուկուսիմա (պրեֆեկտուրա)|Ֆուկուշիմա նահանգում]]&lt;ref name="bauer-patricia-brit" /&gt;&lt;ref name="encyclopedia" /&gt;։ Նա յոթ երեխաներից հինգերորդն էր&lt;ref name="robert-horn-si"/&gt;։ Հայրը հրատարակիչ էր։</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>Ջունկո Իշիբաշին ծնվել է [[1939 թվական]]ի [[սեպտեմբերի 22]]-ին [[Միհարու]]յում՝ [[Ֆուկուսիմա (պրեֆեկտուրա)|Ֆուկուշիմա նահանգում]]&lt;ref name="bauer-patricia-brit" /&gt;&lt;ref name="encyclopedia" /&gt;։ Նա յոթ երեխաներից հինգերորդն էր&lt;ref name="robert-horn-si"/&gt;։ Հայրը հրատարակիչ էր։</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Ջունկոն թույլ երեխա էր համարվում, բայց, <del class="diffchange diffchange-inline">չնայած</del> դրան, մագլցել սկսել է տաս տարեկանից՝ դպրոցի կազմակերպած [[Նասու լեռնագագաթ]]ը <del class="diffchange diffchange-inline">բարձրանալու</del> արշավին մասնակցելով։&lt;ref name="douglas-ed"&gt;{{cite news</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Ջունկոն թույլ երեխա էր համարվում, բայց, <ins class="diffchange diffchange-inline">չնայաց</ins> դրան, մագլցել սկսել է տաս տարեկանից՝ դպրոցի կազմակերպած [[Նասու լեռնագագաթ]]ը <ins class="diffchange diffchange-inline">մագլցելու</ins> արշավին մասնակցելով։&lt;ref name="douglas-ed"&gt;{{cite news</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|last = Դագլաս</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|last = Դագլաս</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|first = Էդ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|first = Էդ</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  109.</td>
          <td colspan="2" class="diff-lineno">Տող  109.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|first = Բիլլ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|first = Բիլլ</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|date = 2016, հոկտեմբերի 22</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|date = 2016, հոկտեմբերի 22</div></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>|title = Ճապոնացի <del class="diffchange diffchange-inline">լեռնագնաց</del> Ջունկո Տաբեյը, Էվերեստ <del class="diffchange diffchange-inline">բարձրացած</del> <del class="diffchange diffchange-inline">առաջին</del> <del class="diffchange diffchange-inline">կինը</del>, մահացել է 77 տարեկանում</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>|title = Ճապոնացի <ins class="diffchange diffchange-inline">Լեռնագնաց</ins> Ջունկո Տաբեյը, Էվերեստ <ins class="diffchange diffchange-inline">Բարձրացած</ins> <ins class="diffchange diffchange-inline">Առաջին</ins> <ins class="diffchange diffchange-inline">Կինը</ins>, մահացել է 77 տարեկանում</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|newspaper = NPR</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|newspaper = NPR</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|url = https://www.npr.org/sections/thetwo-way/2016/10/22/498971169/japanese-climber-junko-tabei-first-woman-to-conquer-mount-everest-dies-at-77</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|url = https://www.npr.org/sections/thetwo-way/2016/10/22/498971169/japanese-climber-junko-tabei-first-woman-to-conquer-mount-everest-dies-at-77</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  115.</td>
          <td colspan="2" class="diff-lineno">Տող  115.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt; Նրան դուր եկավ սպորտի ոչ֊մրցակցային բնույթը ու գագաթից բացվող հոյակապ տեսարանները։ Չնայած Ջունկոն հետաքրքված էր մագլցմամբ ու ուզում էր շարունակել դրանով զբաղվել, ընտանիքը նման թանկ հոբբիի համար բավարար գումար չուներ, ու Իշիբաշին ավագ դպրոցի տարիներին մագցմամբ շատ չի զբաղվել&lt;ref name="robert-horn-si" /&gt;։</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt; Նրան դուր եկավ սպորտի ոչ֊մրցակցային բնույթը ու գագաթից բացվող հոյակապ տեսարանները։ Չնայած Ջունկոն հետաքրքված էր մագլցմամբ ու ուզում էր շարունակել դրանով զբաղվել, ընտանիքը նման թանկ հոբբիի համար բավարար գումար չուներ, ու Իշիբաշին ավագ դպրոցի տարիներին մագցմամբ շատ չի զբաղվել&lt;ref name="robert-horn-si" /&gt;։</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>[[1958]]-ից [[1962]] թվականներին Իշիբաշին [[Շովայի Կանանց Համալսարան]]ում <del class="diffchange diffchange-inline">անգլիական</del> ու <del class="diffchange diffchange-inline">ամերիկյան</del> գրականություն է ուսումնասիրել։ Այդ ժամանակ նրա նպատակը ուսուցիչ դառնալն էր։ Համալսարանն ավարտելուց հետո Ջունկոն վերադառնում է իր առաջին հետաքրությանը՝ մագլցմանը, ու միանում<del class="diffchange diffchange-inline"> տղամարկանց մագլցման</del> մի քանի ակումբների։ Որոշ տղամարդիկ ընդունում ու խրախուսում էին Ջունկոյին՝ որպես իրենց նման մագլցող աթլետի, շատերը՝ քննադատում ու հարցականի տակ<del class="diffchange diffchange-inline"> էին</del> դնում Ջունկոյի տղամարդկանց հատուկ սպորտաձևին միանալու իրական դրդապատճառներն ու նպատակները&lt;ref name="douglas-ed" /&gt;։ </div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>[[1958]]-ից [[1962]] թվականներին Իշիբաշին [[Շովայի Կանանց Համալսարան]]ում <ins class="diffchange diffchange-inline">Անգլիական</ins> ու <ins class="diffchange diffchange-inline">Ամերիկյան</ins> գրականություն է ուսումնասիրել։ Այդ ժամանակ նրա նպատակը ուսուցիչ դառնալն էր։ Համալսարանն ավարտելուց հետո Ջունկոն վերադառնում է իր առաջին հետաքրությանը՝ մագլցմանը, ու միանում մի քանի<ins class="diffchange diffchange-inline"> տղամարկանց մագլցման</ins> ակումբների։ Որոշ տղամարդիկ ընդունում ու խրախուսում էին Ջունկոյին՝ որպես իրենց նման մագլցող աթլետի, շատերը՝ քննադատում<ins class="diffchange diffchange-inline"> էին</ins> ու հարցականի տակ դնում Ջունկոյի տղամարդկանց հատուկ սպորտաձևին միանալու իրական դրդապատճառներն ու նպատակները&lt;ref name="douglas-ed" /&gt;։ </div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>Իշիբաշին շուտով հաղթահարում է Ճապոնիայի բոլոր խոշոր լեռները, ներառյալ [[Ֆուձիյամա|Ֆուջին]]&lt;ref name="robert-horn-si" /&gt;։</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>Իշիբաշին շուտով հաղթահարում է Ճապոնիայի բոլոր խոշոր լեռները, ներառյալ [[Ֆուձիյամա|Ֆուջին]]&lt;ref name="robert-horn-si" /&gt;։</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  127.</td>
          <td colspan="2" class="diff-lineno">Տող  127.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>Տաբեյը իր մագլցումների համար գումար էր աշխատում ''«Journal of the Physical Society of Japan»'' ամսագրի համար խմբագիր աշխատելով։&lt;ref name="robert-horn-si" /&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>Տաբեյը իր մագլցումների համար գումար էր աշխատում ''«Journal of the Physical Society of Japan»'' ամսագրի համար խմբագիր աշխատելով։&lt;ref name="robert-horn-si" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>[[1969 թվական]]ին Տաբեյը հիմնադրում է միայն կանանց համար նախատեսված Ջոշի֊Տոհան ({{lang-jp|女子登攀}})` Կանանց Լեռնագնացության ակումբը։ Ակումբի կարգախոսն էր «եկե՛ք մենք մեզանով գնանք արտերկրյա արշավանքների»&lt;ref name="douglas-ed" /&gt;։ Ակումբը Ճապոնիայում իր տեսակի մեջ միակն էր։ Տաբեյը հետագայում ասել է, որ ակումբի հիմնադրման պատճառը ժամանակի տղամարդ մագլցողների վերաբերմունքն էր իր նկատմամբ։ Որոշ տղամարդիկ, օրինակ, հրաժարվում էին իր հետ մագլցել, որոշները մտածում էին, որ Տաբեյի <del class="diffchange diffchange-inline">մագլցմամբ</del> <del class="diffchange diffchange-inline">հետաքրքրվում էր միայն</del> ամուսին <del class="diffchange diffchange-inline">գտնելու</del> <del class="diffchange diffchange-inline">համար։</del>&lt;ref name="chapell-npr" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>[[1969 թվական]]ին Տաբեյը հիմնադրում է միայն կանանց համար նախատեսված Ջոշի֊Տոհան ({{lang-jp|女子登攀}})` Կանանց Լեռնագնացության ակումբը։ Ակումբի կարգախոսն էր «եկե՛ք մենք մեզանով գնանք արտերկրյա արշավանքների»&lt;ref name="douglas-ed" /&gt;։ Ակումբը Ճապոնիայում իր տեսակի մեջ միակն էր։ Տաբեյը հետագայում ասել է, որ ակումբի հիմնադրման պատճառը ժամանակի տղամարդ մագլցողների վերաբերմունքն էր իր նկատմամբ։ Որոշ տղամարդիկ, օրինակ, հրաժարվում էին իր հետ մագլցել, որոշները մտածում էին, որ Տաբեյի <ins class="diffchange diffchange-inline">մագլցման</ins> <ins class="diffchange diffchange-inline">հետաքրքրությունը</ins> ամուսին <ins class="diffchange diffchange-inline">գտնելն</ins> <ins class="diffchange diffchange-inline">էր։</ins>&lt;ref name="chapell-npr" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Ջոշի֊Տոհան ակումբը իր առաջին արշավանքը կատարել է [[1970 թվական]]ին․ Տաբեյը և մեկ այլ անդամ՝ Հիրոկո Հիրակավան, երկու [[շերպեր|շերպա]] ուղեկցողների հետ մագլցել են [[Նեպալ]]ական [[Աննապուռնա III]] լեռը։ <del class="diffchange diffchange-inline">Օգտագործելով</del> լեռան հարավային կողմով անցնող նոր ճանապարհը&lt;ref name="bauer-patricia-brit" /&gt; նրանք հաջողությամբ հասնում են գագաթին՝ արձանագրելով այդ լեռան կանաց<del class="diffchange diffchange-inline">,</del> <del class="diffchange diffchange-inline">ինչպես</del> <del class="diffchange diffchange-inline">նաև</del> ճապոնացիների առաջին վերելքը։&lt;ref name="derek-franz"&gt;{{cite web</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Ջոշի֊Տոհան ակումբը իր առաջին արշավանքը կատարել է [[1970 թվական]]ին․ Տաբեյը և մեկ այլ անդամ՝ Հիրոկո Հիրակավան, երկու [[շերպեր|շերպա]] ուղեկցողների հետ<ins class="diffchange diffchange-inline">,</ins> մագլցել են [[Նեպալ]]ական [[Աննապուռնա III]] լեռը։ <ins class="diffchange diffchange-inline">Օգտագործելոով</ins> լեռան հարավային կողմով անցնող նոր ճանապարհը&lt;ref name="bauer-patricia-brit" /&gt;<ins class="diffchange diffchange-inline">,</ins> նրանք հաջողությամբ հասնում են գագաթին՝ արձանագրելով այդ լեռան կանաց <ins class="diffchange diffchange-inline">առաջին</ins> <ins class="diffchange diffchange-inline">ու</ins> ճապոնացիների առաջին վերելքը։&lt;ref name="derek-franz"&gt;{{cite web</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|url = http://www.alpinist.com/doc/web16f/newswire-junko-tabei-obit</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|url = http://www.alpinist.com/doc/web16f/newswire-junko-tabei-obit</div></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>|title = Ջունկո <del class="diffchange diffchange-inline">Տաբեյը՝</del> Էվերեստը բարձրացած առաջին կինը, մահանում է 77 տարեկանում</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>|title = Ջունկո <ins class="diffchange diffchange-inline">Տաբեյ՝</ins> Էվերեստը բարձրացած առաջին կինը, մահանում է 77 տարեկանում</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|last = Ֆրանց</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|last = Ֆրանց</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|first = Դերեկ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|first = Դերեկ</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  152.</td>
          <td colspan="2" class="diff-lineno">Տող  152.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Աննապուռնա III-ի մագլցման փորձը ցույց է տալիս Տաբեյին, որ ինքն ու այլ ճապոնացի կանայք երբեմն ավանդական ճապոնական արժեքներն ու լեռնագնացության պրակտիկ պահանջները համադրելու խնդիր ունեն։ Ջոշի֊Տոհանի շատ անդամներ, օրինակ, վարանում էին ընդունել, երբ ինչ֊որ բան չգիտեին կամ օգնության կարիք ունեին՝ նախընտրելով ստոիկ լռություն պահել, բայց լեռնագնացությունը ստիպում է նրանց սեփական սահմնափակումները գիտակցել ու իրարից օգնություն ընդունել։&lt;ref name="robert-horn-si" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Աննապուռնա III<ins class="diffchange diffchange-inline"> </ins>-ի մագլցման փորձը ցույց է տալիս Տաբեյին, որ ինքն ու այլ ճապոնացի կանայք երբեմն ավանդական ճապոնական արժեքներն ու լեռնագնացության պրակտիկ պահանջները համադրելու խնդիր ունեն։ Ջոշի֊Տոհանի շատ անդամներ, օրինակ, վարանում էին ընդունել, երբ ինչ֊որ բան չգիտեին<ins class="diffchange diffchange-inline">,</ins> կամ օգնության կարիք ունեին՝ նախընտրելով ստոիկ լռություն պահել, բայց լեռնագնացությունը ստիպում է նրանց սեփական սահմնափակումները գիտակցել ու իրարից օգնություն ընդունել։&lt;ref name="robert-horn-si" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>==== 1975 թվականի Էվերեստի արշավանքը ====</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>==== 1975 թվականի Էվերեստի արշավանքը ====</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  168.</td>
          <td colspan="2" class="diff-lineno">Տող  168.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>[[1971 թվական]]ին ակումբը Էվերեստը մագլցելու թույլտվության հայտ է ներկայացնում։ Մագլցման պաշտոնական ժամանակացուցակում տեղ ստանալուն թիմը կսպասի չորս տարի։&lt;ref name="robert-horn-si" /&gt;&lt;ref name="encyclopedia" /&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>[[1971 թվական]]ին ակումբը Էվերեստը մագլցելու թույլտվության հայտ է ներկայացնում։ Մագլցման պաշտոնական ժամանակացուցակում տեղ ստանալուն թիմը կսպասի չորս տարի։&lt;ref name="robert-horn-si" /&gt;&lt;ref name="encyclopedia" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Տաբեյը, չնայած հաճախ է լսում, որ «կանայք պետք է (մագլցելու փոխարեն) երեխաներ մեծացնելով զբաղվեն»,&lt;ref name="chapell-npr" /&gt; աշխատում է արշավանքին հովանավորներ գտնելու ուղղությամբ։ Նրան հաջողվում է վերջին պահին «Յոմիուրի Շիմբուն» թերթից և «Նիհոն Տերեբի» հեռուստաալիքից որոշ ֆինանսավորում ստանալ,&lt;ref name="robert-horn-si" /&gt; սակայն այն բավարար չէր բոլոր ծախսերը փակելու համար. խմբի յուրաքանչյուր անդամ դեռ պետք է 1.5 միլիոն [[իեն]] (մոտավորապես 5000 դոլար) վճարեր։&lt;ref name="frenette-brad" /&gt; Տաբեյը անհրաժեշտ դրամական միջոցները հավաքելու համար [[դաշնամուր]] է դասավանդում &lt;ref name="encyclopedia" /&gt; ու որոշում է սեփական սարքավորման ու հագուստի մեծ մասը ինքնուրույն ստեղծել՝ գումար խնայելու համար։ Այդպես, օրինակ․ Տաբեյը իր մեքենայի ծածկից անջրանցիկ ձեռնոցներ է կարում, իսկ հին վարագույրներից՝ շալվարներ։&lt;ref name="padoan-amanda"&gt;{{cite web</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Տաբեյը, չնայած հաճախ է լսում, որ «կանայք պետք է (մագլցելու փոխարեն) երեխաներ մեծացնելով զբաղվեն»,&lt;ref name="chapell-npr" /&gt; աշխատում է արշավանքին հովանավորներ գտնելու ուղղությամբ։ Նրան հաջողվում է վերջին պահին «Յոմիուրի Շիմբուն» թերթից և «Նիհոն Տերեբի» հեռուստաալիքից որոշ ֆինանսավորում ստանալ,&lt;ref name="robert-horn-si" /&gt; սակայն այն բավարար չէր բոլոր ծախսերը փակելու համար. խմբի յուրաքանչյուր անդամ դեռ պետք է 1.5 միլիոն [[իեն]] (մոտավորապես 5000 դոլար) վճարեր։&lt;ref name="frenette-brad" /&gt; Տաբեյը անհրաժեշտ դրամական միջոցները հավաքելու համար [[դաշնամուր]] է դասավանդում<ins class="diffchange diffchange-inline">,</ins> &lt;ref name="encyclopedia" /&gt; ու որոշում է սեփական սարքավորման ու հագուստի մեծ մասը ինքնուրույն ստեղծել՝ գումար խնայելու համար։ Այդպես, օրինակ․ Տաբեյը իր մեքենայի ծածկից անջրանցիկ ձեռնոցներ է կարում, իսկ հին վարագույրներից՝ շալվարներ։&lt;ref name="padoan-amanda"&gt;{{cite web</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|title = Էվերեստը հաղթահարած առաջին կնոջ ոգեշնչող պատմությունը</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|title = Էվերեստը հաղթահարած առաջին կնոջ ոգեշնչող պատմությունը</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|last = Պադոան</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|last = Պադոան</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  179.</td>
          <td colspan="2" class="diff-lineno">Տող  179.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Երկար նախապատրաստումներից ու պարապմունքներից հետո&lt;ref name="otake-tomoko-jt" /&gt;, [[1975 թվական]]ի մայիսին JWEE խումբը սկսում է արշավանքը&lt;ref&gt;{{cite news</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Երկար նախապատրաստումներից ու պարապմունքներից հետո<ins class="diffchange diffchange-inline"> </ins>&lt;ref name="otake-tomoko-jt" /&gt;, [[1975 թվական]]ի մայիսին JWEE խումբը սկսում է արշավանքը&lt;ref&gt;{{cite news</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|last = Ռոբերթս</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|last = Ռոբերթս</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|first = Սեմ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|first = Սեմ</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  200.</td>
          <td colspan="2" class="diff-lineno">Տող  200.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt;։</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt;։</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>[[Մայիսի 4]]֊ին 6300 մետր բարձրության վրա թիմի խփած ճամբարը ընկնում է [[ձնահյուս]]ի տակ։ Տաբեյը և չորս այլ մագլցողներ հայտնվում են ձյան տակ՝ թաղված։ Տաբեյը կորցնում է գիտակցությունը։ Շերպա ուղեկցորդների շները գտնում ու փորելով հանում են նրան։&lt;ref name="frenette-brad" /&gt; Բարեբախտաբար, այս պատահարը զոհեր չի գրանցում&lt;ref name="bauer-patricia-brit" /&gt;։ Տաբեյը, սակայն այնքան վնասվածքներ ու <del class="diffchange diffchange-inline">կապտուկներ</del> է ստանում, որ հազիվ է կարողանում քայլել։ Նա վերականգնման համար ստիպված է լինում երկու օր հանգստանալ։ Վերականգնվելուց անմիջապես հետո Տաբեյը վերսկսում է արշավանքն ու շարունակում է իր թիմին առաջնորդել դեպի լեռան գագաթ։&lt;ref name="robert-horn-si" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>[[Մայիսի 4]]֊ին 6300 մետր բարձրության վրա թիմի խփած ճամբարը ընկնում է [[ձնահյուս]]ի տակ։ Տաբեյը և չորս այլ մագլցողներ հայտնվում են ձյան տակ՝ թաղված։ Տաբեյը կորցնում է գիտակցությունը։ Շերպա ուղեկցորդների շները գտնում ու փորելով հանում են նրան։&lt;ref name="frenette-brad" /&gt; Բարեբախտաբար, այս պատահարը զոհեր չի գրանցում&lt;ref name="bauer-patricia-brit" /&gt;։ Տաբեյը, սակայն այնքան վնասվածքներ ու <ins class="diffchange diffchange-inline">կապտումներ</ins> է ստանում, որ հազիվ է կարողանում քայլել։ Նա վերականգնման համար ստիպված է լինում երկու օր հանգստանալ։ Վերականգնվելուց անմիջապես հետո Տաբեյը վերսկսում է արշավանքն ու շարունակում է իր թիմին առաջնորդել դեպի լեռան գագաթ։&lt;ref name="robert-horn-si" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Չնայած թիմի նախնական ծրագիրը Էվերեստի գագաթ երկու մարդ հասցնելն էր՝ մեկ շերպա գիդի ուղեկցությամբ, [[լեռնային հիվանդություն|լեռնային հիվանդության]] բռնկումը նշանակում է, որ թիմի շերպերը ի վիճակի չեն լինի երկու մագլցողի համար բավարար քանակությամբ թթվածնի շշեր կրել։ <del class="diffchange diffchange-inline">Այդպիսով․</del> միայն մեկ կին կարող էր շարունակել։ Երկար քննարկումներից հետո Էյկո Հիսանոն առաջադրում է Տաբեյի թեկնածությունը՝ մագլցումը շարունակելու ու այն ավարտին հասցնելու համար։&lt;ref name="frenette-brad" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Չնայած թիմի նախնական ծրագիրը Էվերեստի գագաթ երկու մարդ հասցնելն էր՝ մեկ շերպա գիդի ուղեկցությամբ, [[լեռնային հիվանդություն|լեռնային հիվանդության]] բռնկումը նշանակում է, որ թիմի շերպերը ի վիճակի չեն լինի երկու մագլցողի համար բավարար քանակությամբ թթվածնի շշեր կրել։ <ins class="diffchange diffchange-inline">Այդպիսով,</ins> միայն մեկ կին կարող էր շարունակել։ Երկար քննարկումներից հետո Էյկո Հիսանոն առաջադրում է Տաբեյի թեկնածությունը՝ մագլցումը շարունակելու ու այն ավարտին հասցնելու համար։&lt;ref name="frenette-brad" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Գագաթին մոտ Տաբեյը կատաղությամբ բացահայտում է, որ իրեն գագաթից բարակ, վտանգավոր սառույցի շերտ է բաժանում, որի մասին <del class="diffchange diffchange-inline">Էվերեստի</del> նախորդ արշավանքների <del class="diffchange diffchange-inline">նկարագրություններում որևէ հիշատակում չկար։</del> Տաբեյը սառույցի շերտն անցնում է կողքի վրա սողալով ու հետագայում նկարագրում է միջադեպը որպես իր կյանքի ամենալարված պահը։&lt;ref name="otake-tomoko-jt" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Գագաթին մոտ Տաբեյը կատաղությամբ բացահայտում է, որ իրեն գագաթից բարակ, վտանգավոր սառույցի շերտ է բաժանում, որի մասին <ins class="diffchange diffchange-inline">որևէ հիշատակում չկար</ins> նախորդ արշավանքների <ins class="diffchange diffchange-inline">նկարագրություններում։</ins> Տաբեյը սառույցի շերտն անցնում է կողքի վրա սողալով<ins class="diffchange diffchange-inline">,</ins> ու հետագայում նկարագրում է միջադեպը որպես իր կյանքի ամենալարված պահը։&lt;ref name="otake-tomoko-jt" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>Ձնահյուսից 12 օր անց՝ [[1975 թվական]]ի [[մայիսի 16]]֊ին, իր շերպա գիդ Անգ Ցերինգի ուղեկցությամբ, Ջունկո Տաբեյը դառնում է Էվերեստի գագաթը հասած առաջին կինը։&lt;ref name="otake-tomoko-jt" /&gt;&lt;ref name="kumari-sunayana" /&gt; Այս նվաճումը նրան դնում է աշխարհի ուշադրության կենտրոնում։ </div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>Ձնահյուսից 12 օր անց՝ [[1975 թվական]]ի [[մայիսի 16]]֊ին, իր շերպա գիդ Անգ Ցերինգի ուղեկցությամբ, Ջունկո Տաբեյը դառնում է Էվերեստի գագաթը հասած առաջին կինը։&lt;ref name="otake-tomoko-jt" /&gt;&lt;ref name="kumari-sunayana" /&gt; Այս նվաճումը նրան դնում է աշխարհի ուշադրության կենտրոնում։ </div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  213.</td>
          <td colspan="2" class="diff-lineno">Տող  213.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|url = https://www.independent.co.uk/news/world/junko-tabei-google-doodle-today-who-mount-everest-japan-death-a9115076.html</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|url = https://www.independent.co.uk/news/world/junko-tabei-google-doodle-today-who-mount-everest-japan-death-a9115076.html</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|title = Ջունկո Տաբեյ․ Գուգլ Դուդլը տոնում է Էվերեստը բարձրացած առաջին կնոջ հիշատակը</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|title = Ջունկո Տաբեյ․ Գուգլ Դուդլը տոնում է Էվերեստը բարձրացած առաջին կնոջ հիշատակը</div></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>|newspaper = Independent UK </div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>|newspaper = Independent UK <ins class="diffchange diffchange-inline">|</ins></div></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div><del class="diffchange diffchange-inline">|</del>date = 2019, սեպտեմբերի 22</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>date = 2019, սեպտեմբերի 22</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|first = Քրիս</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|first = Քրիս</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|last = Բայնզ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|last = Բայնզ</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  222.</td>
          <td colspan="2" class="diff-lineno">Տող  222.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>==== Ուշ շրջանի գործունեությունը ====</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>==== Ուշ շրջանի գործունեությունը ====</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Տաբեյը շարունակում է մագլցել, ի վերջո բարձրանալով յուրաքանչյուր մայրցամաքի ամենաբարձր կետը․ [[Կիլիմանջարո]] (1980), [[Ակոնկագուա]] (1987), [[Դենալի]] (1988), [[Էլբրուս]] (1989), [[Վինսոն լեռնազանգված|Վինսոն]] (1991) և [[Պունջակ Ջայա]] (1992)<del class="diffchange diffchange-inline">։</del>&lt;ref name="bauer-patricia-brit" /&gt; Պունջակ Ջայայի բարեհաջող մագլցումից հետո Ջունկո Տաբեյը դառնում է աշխարհի [[Յոթ Բարձրունքներ]]ը հաղթահարած առաջին կինը։&lt;ref name="robert-horn-si" /&gt;&lt;ref name="encyclopedia" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Տաբեյը շարունակում է մագլցել, ի վերջո բարձրանալով յուրաքանչյուր մայրցամաքի ամենաբարձր կետը․ [[Կիլիմանջարո]] (1980), [[Ակոնկագուա]] (1987), [[Դենալի]] (1988), [[Էլբրուս]] (1989), [[Վինսոն լեռնազանգված|Վինսոն]] (1991)<ins class="diffchange diffchange-inline">,</ins> և [[Պունջակ Ջայա]] (1992)<ins class="diffchange diffchange-inline">.</ins>&lt;ref name="bauer-patricia-brit" /&gt; Պունջակ Ջայայի բարեհաջող մագլցումից հետո<ins class="diffchange diffchange-inline">,</ins> Ջունկո Տաբեյը դառնում է աշխարհի [[Յոթ Բարձրունքներ]]ը հաղթահարած առաջին կինը։&lt;ref name="robert-horn-si" /&gt;&lt;ref name="encyclopedia" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>Մինչև [[2005 թվական]]ը Տաբեյը մասնակցում է 44 լեռնագնացային արշավանքների ամբողջ աշխարհում՝ միայն կանանցից բաղկացած թիմերով։ Նրա անձնական նպատակը աշխարհի յուրաքանչյուր երկրի ամենաբարձր կետը մագլցելն էր։ Իր կյանքի ընթացքում Տաբեյը կարողանում է այդ լեռներից 70֊ը նվաճել։&lt;ref name="bauer-patricia-brit" /&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>Մինչև [[2005 թվական]]ը Տաբեյը մասնակցում է 44 լեռնագնացային արշավանքների ամբողջ աշխարհում՝ միայն կանանցից բաղկացած թիմերով։ Նրա անձնական նպատակը աշխարհի յուրաքանչյուր երկրի ամենաբարձր կետը մագլցելն էր։ Իր կյանքի ընթացքում Տաբեյը կարողանում է այդ լեռներից 70֊ը նվաճել։&lt;ref name="bauer-patricia-brit" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Էվերեստը մագլցելուց հետո Ջունկո Տաբեյը երբեք կորպորատիվ հովանավորություն չի ընդունում, նախընտրելով ֆինանսապես անկախ մնալ։ Նա իր արշավանքների համար գումար է աշխատում վճարովի հանրային ելույթներով, լեռնամագլցային արշավանքներ առաջնորդելով ու երեխաներին անգլերենի ու երաժշտության անհատական դասեր <del class="diffchange diffchange-inline">տալով։ Տաբեյի</del> ընկերներն ու աջակիցները երբեմն նրան ուտելիք ու սարքավորումներ են նվիրաբերում։&lt;ref&gt;{{cite news </div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Էվերեստը մագլցելուց հետո Ջունկո Տաբեյը երբեք կորպորատիվ հովանավորություն չի ընդունում, նախընտրելով ֆինանսապես անկախ մնալ։ Նա իր արշավանքների համար գումար է աշխատում վճարովի հանրային ելույթներով, լեռնամագլցային արշավանքներ առաջնորդելով ու երեխաներին անգլերենի ու երաժշտության անհատական դասեր <ins class="diffchange diffchange-inline">տալով։Տաբեյի</ins> ընկերներն ու աջակիցները երբեմն նրան ուտելիք ու սարքավորումներ են նվիրաբերում։&lt;ref&gt;{{cite news </div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|last = Կուրտենբախ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|last = Կուրտենբախ</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|first = Իլեյն</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|first = Իլեյն</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  237.</td>
          <td colspan="2" class="diff-lineno">Տող  237.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Մագլցման զուգահեռ Տաբեյը աշխատանք է տարել բնապահպանության ոլորտում։ [[2000 թվական]]ին նա ավարտում է Քյուշուի համալսարանի ասպիրատուրան՝ մագլցման խմբերի թողած աղբի հետևանքով Էվերեստի շրջակա միջավայրի քայքայման մասին <del class="diffchange diffchange-inline">ուսումնասիրությամբ։</del> Տաբեյը նաև Ճապոնիայի Հիմալայան Արկածների կենտրոնի՝ միջազգային մակարդակով լեռնային միջավայրերի պահպանությամբ զբաղվող կազմակերպության տնօրենն էր։&lt;ref name="otake-tomoko-jt" /&gt; Կենտրոնի ծրագրերից մեկը մագլցողների<del class="diffchange diffchange-inline"> թողած</del> աղբի հրկիզման համար <del class="diffchange diffchange-inline">արդյունաբերական</del> վառարան կառուցելն էր։ Տաբեյը նաև իր ամուսնու ու երեխաների հետ մասնակցել է «աղբահավաք» <del class="diffchange diffchange-inline">լեռնային արշավների՝</del> Հիմալայներում ու Ճապոնիայում։&lt;ref name="robert-horn-si" /&gt;&lt;ref name="encyclopedia" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Մագլցման զուգահեռ<ins class="diffchange diffchange-inline">,</ins> Տաբեյը աշխատանք է տարել բնապահպանության ոլորտում։ [[2000 թվական]]ին նա ավարտում է Քյուշուի համալսարանի ասպիրատուրան՝ մագլցման խմբերի թողած աղբի հետևանքով Էվերեստի շրջակա միջավայրի քայքայման մասին <ins class="diffchange diffchange-inline">ուսումնասիրությունների կենտրոնացմամբ։</ins> Տաբեյը նաև Ճապոնիայի Հիմալայան Արկածների կենտրոնի՝ միջազգային մակարդակով լեռնային միջավայրերի պահպանությամբ զբաղվող կազմակերպության տնօրենն էր։&lt;ref name="otake-tomoko-jt" /&gt; Կենտրոնի ծրագրերից մեկը մագլցողների աղբի հրկիզման համար <ins class="diffchange diffchange-inline">արյունաբերական</ins> վառարան կառուցելն էր։ Տաբեյը նաև իր ամուսնու ու երեխաների հետ մասնակցել է «աղբահավաք» <ins class="diffchange diffchange-inline">մագլցումների՝</ins> Հիմալայներում ու Ճապոնիայում։&lt;ref name="robert-horn-si" /&gt;&lt;ref name="encyclopedia" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>[[2003 թվական]]ի մայիսին [[Կատմանդու]]ում Էվերեստը բարձրանալու առաջին հաջող փորձի 50֊րդ տարելիցի տոնակատարություններ են անց կացվել։ Նեպալի ժողովուրդը հավաքվել էր՝ Էվերեստը մագլցած մարդկանց տեսնելու ու նրանց հաջողությունները տոնելու։ Ջունկո Տաբեյը և Էդմունդ Հիլարին հատուկ տեղ ունեին շքերթում՝ իրենց համապատասխան ձեռքբերումների պատվին։&lt;ref&gt;{{cite web</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>[[2003 թվական]]ի մայիսին [[Կատմանդու]]ում Էվերեստը բարձրանալու առաջին հաջող փորձի 50֊րդ տարելիցի տոնակատարություններ են անց կացվել։ Նեպալի ժողովուրդը հավաքվել էր՝ Էվերեստը մագլցած մարդկանց տեսնելու ու նրանց հաջողությունները տոնելու։ Ջունկո Տաբեյը և Էդմունդ Հիլարին հատուկ տեղ ունեին շքերթում՝ իրենց համապատասխան ձեռքբերումների պատվին։&lt;ref&gt;{{cite web</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  248.</td>
          <td colspan="2" class="diff-lineno">Տող  248.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>}}&lt;/ref&gt;</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>}}&lt;/ref&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>[[1996]]-ից [[2008]] թվականներին Տաբեյը յոթ գիրք է <del class="diffchange diffchange-inline">գրում</del> ու <del class="diffchange diffchange-inline">հրատարակում։</del> [[2011 թվական]]ի [[Երկրաշարժ Ճապոնիայում (2011)|երկրաշարժից]] հետո Տաբեյը արհավիրքրց տուժած դպրոցականների համար ամենամյա արշավանք է <del class="diffchange diffchange-inline">կազմակերպում</del> դեպի Ֆուջի լեռ։&lt;ref name="derek-franz" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>[[1996]]-ից [[2008]] թվականներին<ins class="diffchange diffchange-inline">,</ins> Տաբեյը յոթ գիրք է <ins class="diffchange diffchange-inline">գրել</ins> ու <ins class="diffchange diffchange-inline">հրատարակել։</ins> [[2011 թվական]]ի [[Երկրաշարժ Ճապոնիայում (2011)|երկրաշարժից]] հետո Տաբեյը արհավիրքրց տուժած դպրոցականների համար ամենամյա արշավանք է <ins class="diffchange diffchange-inline">կազմակերպում՝</ins> դեպի Ֆուջի լեռ։&lt;ref name="derek-franz" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>=== Մահը և Ժառանգությունը ===</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>=== Մահը և Ժառանգությունը ===</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>[[2012 թվական]]ին Տաբեյը ստանում է [[ստամոքսի քաղցկեղ]]ի ախտորոշում, բայց շարունակում է իր լեռնագցային գործունեությունը։ [[2016 թվական]]ին, չնայած հիվանդության առաջխաղացմանը, Տաբեյը երիտասարդների խումբ է առաջնորդում Ֆուջի <del class="diffchange diffchange-inline">լեռ։</del>&lt;ref name="douglas-ed" /&gt;</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>[[2012 թվական]]ին Տաբեյը ստանում է [[ստամոքսի քաղցկեղ]]ի ախտորոշում, բայց շարունակում է իր լեռնագցային գործունեությունը։ [[2016 թվական]]ին, չնայած հիվանդության առաջխաղացմանը, Տաբեյը երիտասարդների խումբ է առաջնորդում Ֆուջի <ins class="diffchange diffchange-inline">լեռ արշավանքի համար։</ins>&lt;ref name="douglas-ed" /&gt;</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>Տաբեյը մահանում է [[2016]] թվականի [[հոկտեմբերի 16]]-ին՝ Կավագոեում։&lt;ref name="chapell-npr" /&gt; </div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>Տաբեյը մահանում է [[2016]] թվականի [[հոկտեմբերի 16]]-ին՝ Կավագոեում։&lt;ref name="chapell-npr" /&gt; <ins class="diffchange diffchange-inline">[[(6897) Տաբեյ]] [[աստերոիդ]]ը իր անունը ստանում է Ջունկո Տաբեյի պատվին նախքան մահը&lt;ref name="padoan-amanda" /&gt;։</ins></div></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><br /></td>
          <td colspan="2" class="diff-empty diff-side-added"></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>[[(6897) Տաբեյ]] [[աստերոիդ]]ը իր անունը ստանում է Ջունկո Տաբեյի պատվին&lt;ref name="padoan-amanda" /&gt;։</div></td>
          <td colspan="2" class="diff-empty diff-side-added"></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>[[2019 թվական]]ի [[սեպտեմբերի 22]]ին, [[Գուգլ]]ը նշել է Տաբեյի 80-րդ տարեդարձի օրը [[Google Doodle|դուդլով]]։ Դուդլին ուղեկցող շարադրությունը Տաբեյի մոտիվացիոն կարգախոսն է մեջ բերել․ «Չհանձնվե՛ս։ Շարունակի՛ր դեպի նպատակդ»&lt;ref&gt;{{cite web</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>[[2019 թվական]]ի [[սեպտեմբերի 22]]ին, [[Գուգլ]]ը նշել է Տաբեյի 80-րդ տարեդարձի օրը [[Google Doodle|դուդլով]]։ Դուդլին ուղեկցող շարադրությունը Տաբեյի մոտիվացիոն կարգախոսն է մեջ բերել․ «Չհանձնվե՛ս։ Շարունակի՛ր դեպի նպատակդ»&lt;ref&gt;{{cite web</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  271.</td>
          <td colspan="2" class="diff-lineno">Տող  269.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|website = Միջազգային Աստրոնոմիկայի Միության Մոլորակային Անվանակարգերի Թերթ</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|website = Միջազգային Աստրոնոմիկայի Միության Մոլորակային Անվանակարգերի Թերթ</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|access-date = 2020, հունվարի 20</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|access-date = 2020, հունվարի 20</div></td>
        </tr>
        <tr>
          <td class="diff-marker" data-marker="−"></td>
          <td class="diff-deletedline diff-side-deleted"><div>}}&lt;/ref&gt;։ Պլուտոնի վրա լեռների <del class="diffchange diffchange-inline">անունները</del> ավանդաբար հիշատակում են «երկրագնդի, ծովերի ու երկնքի հետախուզություններում նոր հորիզոններ կտրած պատմական առաջիններին»&lt;ref&gt;{{cite web</div></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><div>}}&lt;/ref&gt;։ Պլուտոնի վրա լեռների <ins class="diffchange diffchange-inline">անունների ընտրությունը</ins> ավանդաբար հիշատակում են «երկրագնդի, ծովերի ու երկնքի հետախուզություններում նոր հորիզոններ կտրած պատմական առաջիններին»&lt;ref&gt;{{cite web</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|url=https://planetarynames.wr.usgs.gov/Page/Categories</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|url=https://planetarynames.wr.usgs.gov/Page/Categories</div></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>|title = Մոլորակների ու Արբանյակների Հատկանիշների Անվանումների Կատեգորիաներ (Թեմաներ)</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>|title = Մոլորակների ու Արբանյակների Հատկանիշների Անվանումների Կատեգորիաներ (Թեմաներ)</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-lineno">Տող  299.</td>
          <td colspan="2" class="diff-lineno">Տող  297.</td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>== Ծանոթագրություններ ==</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>== Ծանոթագրություններ ==</div></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td colspan="2" class="diff-empty diff-side-deleted"></td>
          <td class="diff-marker" data-marker="+"></td>
          <td class="diff-addedline diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><br /></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><br /></td>
        </tr>
        <tr>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-deleted"><div>{{ծանցանկ}}</div></td>
          <td class="diff-marker"></td>
          <td class="diff-context diff-side-added"><div>{{ծանցանկ}}</div></td>
        </tr>

        """;
    
    static string HtmlContnet2 = """
                     <tr>
                       <td colspan="2" class="diff-lineno">Տող 6.</td>
                       <td colspan="2" class="diff-lineno">Տող 6.</td>
                     </tr>
                     <tr>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-deleted">
                         <div>| չափ = </div>
                       </td>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-added">
                         <div>| չափ = </div>
                       </td>
                     </tr>
                     <tr>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-deleted">
                         <div>| նկարագրում = Ջունկո Տաբեյը [[1985 թվական]]ին [[Իսմիաիլ Սամանի գագաթ]]ին [[Տաջիկիստան]]ում</div>
                       </td>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-added">
                         <div>| նկարագրում = Ջունկո Տաբեյը [[1985 թվական]]ին [[Իսմիաիլ Սամանի գագաթ]]ին [[Տաջիկիստան]]ում</div>
                       </td>
                     </tr>
                     <tr>
                       <td colspan="2" class="diff-empty diff-side-deleted"></td>
                       <td class="diff-marker" data-marker="+"></td>
                       <td class="diff-addedline diff-side-added">
                         <div>| ծննդյան օր = {{birth date|df=yes|1939|9|22}}</div>
                       </td>
                     </tr>
                     <tr>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-deleted">
                         <div>| ծննդավայր = [[Միհարու, Ֆուկուշիմա|Միհարու]], [[Ճապոնական կայսրություն]]</div>
                       </td>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-added">
                         <div>| ծննդավայր = [[Միհարու, Ֆուկուշիմա|Միհարու]], [[Ճապոնական կայսրություն]]</div>
                       </td>
                     </tr>
                     <tr>
                       <td colspan="2" class="diff-empty diff-side-deleted"></td>
                       <td class="diff-marker" data-marker="+"></td>
                       <td class="diff-addedline diff-side-added">
                         <div>| վախճանի օր = {{death date and age |2016|10|20 |1939|09|22 |df=yes}}</div>
                       </td>
                     </tr>
                     <tr>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-deleted">
                         <div>| վախճանի վայրը = [[Կավագոե, Սայթամա|Կավագոե]], [[Ճապոնիա]]</div>
                       </td>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-added">
                         <div>| վախճանի վայրը = [[Կավագոե, Սայթամա|Կավագոե]], [[Ճապոնիա]]</div>
                       </td>
                     </tr>
                     <tr>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-deleted">
                         <div>| քաղաքացիություն = </div>
                       </td>
                       <td class="diff-marker"></td>
                       <td class="diff-context diff-side-added">
                         <div>| քաղաքացիություն = </div>
                       </td>
                     </tr>
                     """;
    public StatusBarViewModel StatusBarViewModel { get; }

    public MakeListViewModel MakeListViewModel { get; }
    public OptionsViewModel OptionsViewModel { get; }
    public MoreViewModel MoreViewModel { get; }
    public MenuViewModel MenuViewModel { get; set; }
    public DisambigViewModel DisambigViewModel { get; }
    public SkipViewModel SkipViewModel { get; }
    public StartViewModel StartViewModel { get; }

    [Reactive] public ObservableCollection<DiffRow> Diffs { get; set; }

    public EditBoxViewModel EditBoxViewModel { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public WhatLinksHereViewModel WhatLinksHereViewModel { get; }
    public LogsViewModel LogsViewModel { get; }
    public PageLogsViewModel PageLogsViewModel { get; }

    public MainWindowViewModel(
        StatusBarViewModel statusBarViewModel,
        MakeListViewModel makeListViewModel,
        OptionsViewModel optionsViewModel,
        MoreViewModel moreViewModel,
        MenuViewModel menuViewModel,
        DisambigViewModel disambigViewModel,
        SkipViewModel skipViewModel,
        StartViewModel startViewModel,
        EditBoxViewModel editBoxViewModel,
        HistoryViewModel historyViewModel,
        WhatLinksHereViewModel whatLinksHereViewModel,
        LogsViewModel logsViewModel,
        PageLogsViewModel pageLogsViewModel)
    {
        StatusBarViewModel = statusBarViewModel;
        MakeListViewModel = makeListViewModel;
        OptionsViewModel = optionsViewModel;
        MoreViewModel = moreViewModel;
        MenuViewModel = menuViewModel;
        DisambigViewModel = disambigViewModel;
        SkipViewModel = skipViewModel;
        StartViewModel = startViewModel;
        EditBoxViewModel = editBoxViewModel;
        HistoryViewModel = historyViewModel;
        WhatLinksHereViewModel = whatLinksHereViewModel;
        LogsViewModel = logsViewModel;
        PageLogsViewModel = pageLogsViewModel;
        
        var sv = new WikiCompareResultParserService();

        Diffs = sv.ParseCompareHtml(HtmlContnet).ToObservableCollection();
    }
}
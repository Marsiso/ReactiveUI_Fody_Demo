using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUI_Fody_Demo.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        /*
         * Attribute [Reactive]
         * 
         * Atribut [Reactive] slouží při kompilaci kódu v jazyce C# X.X do frameworku .NET X.X
         * jako dodatečný argument pro kompilátor, který při překladu do implementace property objektu
         * inzertuje dodatečné instrukce, které slouží ke komunikaci mezi View a ViewModelem v případě
         * změny fieldu objektu.
         *      Atribut lze nahradit například voláním metody třídy ReactiveObject RaiseOnPropertyChanged(*arguments*)
         * či implementací rozhrání INotifyPropertyChanged.
         * Atribut za nás řeší redundantní volání výše zmíněných alternativ, což vede k lepší čitelnosti kódu a nižší redundanci kódu.
         */
        [Reactive] internal int LeftOperand { get; set; }

        [Reactive] internal int RightOperand { get; set; }

        [Reactive] internal int Result { get; set; }

        internal void Sum () => Result = LeftOperand + RightOperand;
        
        /*
         * Attribute [ObservableAsPropertyHelper]
         * 
         * Atribut [ObservableAsPropertyHelper] obdobně jako při atributu [Reactive] při jeho zkompilování dojde k inzerci kódu do property fieldu objektu,
         * který slouží ke komunikaci ViewModelu s View s rozdílem, že komunikace je jednosměrná a property objektu má definovaný pouze getter, nikoliv setter.
         * Hodnota fieldu objektu bývá běžně závislá na jiných proměných, které představují paramatery určitého vztahu, jehož výsledek odpovídá hodnotě fieldu objektu.
         *      Na příkaldech níže můžeme vidět dva způsoby zápisu, prvním příkladem je kombinace auto-property a atributu,
         * druhým příkaldem je full-property, u níž je použit datový typ ObservableAsPropertyHelper<T>.
         * Na příkaldech níže je hodnota vázána na proměnných levý a pravý operand, jejichž hodnoty jsou sečteny a výsledek předán fieldu objektu.
         */

        /*
         * První příklad:
         * Jedná se o auto-property v kombinaci s atributem [ObservableAsProperty].
         * Jelikož nemáme přístup k fieldu, který bude vygenerován při kompilaci, předání výsledku závislosti realizujeme
         * metodou ToPropertyEx(*args*).
         * Samotný vztah definuji u obou příkladů metodou WhenAny(*args*), která se volá při změně jednotlivých parametrů.
         * Implementaci vztahů nalezneme v konstruktoru třídy níže u příkladu číslo 2.
         */
        internal int ResultDuplicate { [ObservableAsProperty] get; }
        
        /*
         * Druhý příklad:
         * Nyní full-property v kombinaci s datovým typem fieldu ObservableAsPropertyHelper<T>.
         * Datový typ patří mezi generické kolekce, tudíž mu lze předat libovolný datový typ jako argument.
         * S přímým přístupem k fieldu můžeme výsledek vztahu napřímo předat fieldu objektu (klíčové slovo out) metodou ToProperty(*args*).
         */
        private ObservableAsPropertyHelper<int> resultDuplicateDuplicate;

        internal int ResultDuplicateDuplicate => resultDuplicateDuplicate.Value;

        internal MainWindowViewModel()
        {
            ResultDuplicate = 0;
            
            this.WhenAnyValue(x => x.LeftOperand, x => x.RightOperand,
                (l, r) => l + r).ToPropertyEx(this, x => x.ResultDuplicate);
            this.WhenAnyValue(x => x.LeftOperand, x => x.RightOperand,
                (l, r) => l + r).ToProperty(this, x => x.ResultDuplicateDuplicate, out resultDuplicateDuplicate);
        }

        /*
         * Datový typ ObservableCollection<T>
         * 
         * Tento datavý typ umožnuje sledovat změny objektu, který představuje generickou kolekci dat, ViewModelu a v případě jeho změny
         * vyvolat údalost o změně objektu a upozornit odběratele na dotyčné změny.
         * ObservableCollection<T> patří mezi dynamickou kolekci dat.
         * Upozornění na změny vzniká při přidání či odebraní elementu seznamu nebo jeho obnovení.
         * Datový typ patří mezi generické kolekce.
         * Datový typ již má v sobě implementované rozhrání INotifyPropertyChanged pro vyvolání události v případě změny,
         * které vedou k upozornění odběratele.
         * Používá se pro komunikaci mezi View a modelem dat (ViewModel).
         */

        internal record ResultItems(int LeftOperand, int RightOperand, int Result);

        [Reactive] internal ResultItems? SelectedResult { get; set; }
        
        internal ObservableCollection<ResultItems?> Results { get; } = new();

        internal void AddResult() => Results.Add(new (LeftOperand, RightOperand, ResultDuplicate));
        
        internal void RemoveResult()
        {
            Results.Remove(SelectedResult);
            SelectedResult = Results.FirstOrDefault();
        }
    }
}
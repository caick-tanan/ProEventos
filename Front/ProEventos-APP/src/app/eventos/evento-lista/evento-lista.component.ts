import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/Services/evento.service';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  modalRef = {} as BsModalRef;
  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];

  public widthImg = 150;
  public marginImg = 2;
  public showImg = true;

  private filtroListado = '';

  public get filtroLista(): string {
    return this.filtroListado;
  }

  public set filtroLista(value: string){
    this.filtroListado = value;
    // tslint:disable-next-line: max-line-length
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos; // caso possua valor vou passar o filtroLista para o filtrarEventos
  }

  filtrarEventos(filtrarPor: string): Evento[] {
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: any) => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||
      evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
      );
    }

    constructor(
      private eventoService: EventoService,
      private modalService: BsModalService,
      private toastr: ToastrService,
      private spinner: NgxSpinnerService,
      private router: Router
      ) { }

      ngOnInit(): void {
        this.spinner.show();
        this.GetEventos();
      }

      alterarImagem(): void{
        this.showImg = !this.showImg; // vai alterar o estado da imagem caso clique no botão
      }

      public GetEventos(): void {
        this.eventoService.getEventos().subscribe({
          next : (eventos: Evento[]) => {
            this.eventos = eventos;
            this.eventosFiltrados = this.eventos;
          },
          error: (error: any) => {
            this.spinner.hide();
            this.toastr.error('Erro ao Carregar os Eventos', 'Erro!');
          },
          complete: () => this.spinner.hide()
        });
      }

      openModal(template: TemplateRef<any>): void {
        this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
      }

      confirm(): void {
        this.modalRef.hide();
        this.toastr.success('O Evento foi deletado com Sucesso.', 'Deletado!');
      }

      decline(): void {
        this.modalRef.hide();
      }

      detalheEvento(id: number): void{
        this.router.navigate([`eventos/detalhe/${id}`]);
      }
}

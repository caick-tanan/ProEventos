import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-titulo',
  templateUrl: './titulo.component.html',
  styleUrls: ['./titulo.component.scss']
})
export class TituloComponent implements OnInit {

  @Input() titulo!: string;
  @Input() iconClass = 'fa fa-user';
  @Input() subtitulo = 'desde 2022';
  @Input() botaoListar = false;

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  listar(): void {
    // tslint:disable-next-line: max-line-length
    this.router.navigate([`/${this.titulo.toLocaleLowerCase()}/lista`]); // Esse método vai fazer com que ao clicar no botão encaminhe para a rota listar eventos
  }

}

import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PublicationService } from '../services/publication.service';
import { PublishResult } from '../models/publish-result.model';
import { Publication } from '../models/publication.model';

@Component({
  selector: 'app-publish-modal',
  templateUrl: './publish-modal.component.html',
  styleUrls: ['./publish-modal.component.css']
})
export class PublishModalComponent implements OnInit {

  @ViewChild("content", { static: true })
  public content: NgbModal;

  @Input()
  public publishForms: FormGroup[];

  public bundleInfo: FormGroup;

  public publishing = false;
  public requireBundleInfo = false;
  public publishResult: PublishResult = undefined;

  public languages = ["Not applicable","Abkhaz","Afar","Afrikaans","Akan","Albanian","Amharic","Arabic","Aragonese","Armenian","Assamese","Avaric","Avestan","Aymara","Azerbaijani","Bambara","Bashkir","Basque","Belarusian","Bengali, Bangla","Bihari","Bislama","Bosnian","Breton","Bulgarian","Burmese","Catalan,Valencian","Chamorro","Chechen","Chichewa, Chewa, Nyanja","Chinese","Chuvash","Cornish","Corsican","Cree","Croatian","Czech","Danish","Divehi, Dhivehi, Maldivian","Dutch","Dzongkha","English","Esperanto","Estonian","Ewe","Faroese","Fijian","Finnish","French","Fula, Fulah, Pulaar, Pular","Galician","Georgian","German","Greek (modern)","Guaraní","Gujarati","Haitian, Haitian Creole","Hausa","Hebrew (modern)","Herero","Hindi","Hiri Motu","Hungarian","Interlingua","Indonesian","Interlingue","Irish","Igbo","Inupiaq","Ido","Icelandic","Italian","Inuktitut","Japanese","Javanese","Kalaallisut, Greenlandic","Kannada","Kanuri","Kashmiri","Kazakh","Khmer","Kikuyu, Gikuyu","Kinyarwanda","Kyrgyz","Komi","Kongo","Korean","Kurdish","Kwanyama, Kuanyama","Latin","Luxembourgish, Letzeburgesch","Ganda","Limburgish, Limburgan, Limburger","Lingala","Lao","Lithuanian","Luba-Katanga","Latvian","Manx","Macedonian","Malagasy","Malay","Malayalam","Maltese","Māori","Marathi (Marāṭhī)","Marshallese","Mixtepec Mixtec","Mongolian","Nauru","Navajo, Navaho","Northern Ndebele","Nepali","Ndonga","Norwegian Bokmål","Norwegian Nynorsk","Norwegian","Nuosu","Southern Ndebele","Occitan","Ojibwe, Ojibwa","Old Church Slavonic,Church Slavonic,Old Bulgarian","Oromo","Oriya","Ossetian, Ossetic","Panjabi, Punjabi","Pāli","Persian (Farsi)","Polish","Pashto, Pushto","Portuguese","Quechua","Romansh","Kirundi","Romanian","Russian","Sanskrit (Saṁskṛta)","Sardinian","Sindhi","Northern Sami","Samoan","Sango","Serbian","Scottish Gaelic, Gaelic","Shona","Sinhala, Sinhalese","Slovak","Slovene","Somali","Southern Sotho","Spanish, Castilian","Sundanese","Swahili","Swati","Swedish","Tamil","Telugu","Tajik","Thai","Tigrinya","Tibetan Standard, Tibetan, Central","Turkmen","Tagalog","Tswana","Tonga (Tonga Islands)","Turkish","Tsonga","Tatar","Twi","Tahitian","Uyghur, Uighur","Ukrainian","Urdu","Uzbek","Venda","Vietnamese","Volapük","Walloon","Welsh","Wolof","Western Frisian","Xhosa","Yiddish","Yoruba","Zhuang, Chuang","Zulu"];

  constructor(private modalService: NgbModal,
    private publicationService: PublicationService,
    private formBuilder: FormBuilder) {
    this.bundleInfo = this.formBuilder.group({
      ps: ['b2share', Validators.required],
      token: ['', Validators.required],
      title: ['', Validators.required],
      authors: ['', Validators.required],
      contributors: '',
      description: ['', Validators.required],
      open_access: [true, Validators.required],
      type: ['software', Validators.required],
      date: [''],
      subject: '',
      language: '',
      related: '',
      license: '',
      keywords: '',
    });
  }

  ngOnInit() {
  }

  public openModal() {
    this.publishResult = undefined;
    this.modalService.open(this.content, { ariaLabelledBy: 'modal-basic-title', size: 'lg' });
  }

  public publishSingleRepository(publishForm: FormGroup) {
    this.publishing = true;
    this.openModal();

    let publication = this.publicationService.createPublication(publishForm);
    this.publicationService.publishRepository(publication).
      then(result => {
        this.publishing = false;
        this.publishResult = result;
      });
  }

  public openMultipleRepositories() {
    this.requireBundleInfo = true;
    this.openModal();
  }

  public publishMultipleRepositories() {
    this.publishing = true;
    this.requireBundleInfo = false;

    let publications: Publication[] = [];

    for (let i = 0; i < this.publishForms.length; i++) {
      let publishForm = this.publishForms[i];
      publications.push(this.publicationService.createPublication(publishForm));
    }

    publications.push(this.publicationService.createPublication(this.bundleInfo));

    this.publicationService.publishMultipleRepositories(publications)
      .then(result => {
        this.publishing = false;
        this.publishResult = result;
      });
  }
}

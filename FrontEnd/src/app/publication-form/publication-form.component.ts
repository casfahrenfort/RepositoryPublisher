import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { RepoTree, RepoCommit } from '../models/repo-tree.model';
import { RepoService } from '../services/repo.service';
import { PublishModalComponent } from '../publish-modal/publish-modal.component';

@Component({
  selector: 'app-publication-form',
  templateUrl: './publication-form.component.html',
  styleUrls: ['./publication-form.component.css']
})
export class PublicationFormComponent implements OnInit {

  @Input()
  public publishForm: FormGroup;

  @Input() 
  formIndex: number;

  @Input()
  totalForms: number;

  public repoTree: RepoTree;
  public selectedCommit: RepoCommit;

  public loadingRepoTree = false;
  public loadingRepoTreeError = false;

  public languages = ["Abkhaz","Afar","Afrikaans","Akan","Albanian","Amharic","Arabic","Aragonese","Armenian","Assamese","Avaric","Avestan","Aymara","Azerbaijani","Bambara","Bashkir","Basque","Belarusian","Bengali, Bangla","Bihari","Bislama","Bosnian","Breton","Bulgarian","Burmese","Catalan,Valencian","Chamorro","Chechen","Chichewa, Chewa, Nyanja","Chinese","Chuvash","Cornish","Corsican","Cree","Croatian","Czech","Danish","Divehi, Dhivehi, Maldivian","Dutch","Dzongkha","English","Esperanto","Estonian","Ewe","Faroese","Fijian","Finnish","French","Fula, Fulah, Pulaar, Pular","Galician","Georgian","German","Greek (modern)","Guaraní","Gujarati","Haitian, Haitian Creole","Hausa","Hebrew (modern)","Herero","Hindi","Hiri Motu","Hungarian","Interlingua","Indonesian","Interlingue","Irish","Igbo","Inupiaq","Ido","Icelandic","Italian","Inuktitut","Japanese","Javanese","Kalaallisut, Greenlandic","Kannada","Kanuri","Kashmiri","Kazakh","Khmer","Kikuyu, Gikuyu","Kinyarwanda","Kyrgyz","Komi","Kongo","Korean","Kurdish","Kwanyama, Kuanyama","Latin","Luxembourgish, Letzeburgesch","Ganda","Limburgish, Limburgan, Limburger","Lingala","Lao","Lithuanian","Luba-Katanga","Latvian","Manx","Macedonian","Malagasy","Malay","Malayalam","Maltese","Māori","Marathi (Marāṭhī)","Marshallese","Mixtepec Mixtec","Mongolian","Nauru","Navajo, Navaho","Northern Ndebele","Nepali","Ndonga","Norwegian Bokmål","Norwegian Nynorsk","Norwegian","Nuosu","Southern Ndebele","Occitan","Ojibwe, Ojibwa","Old Church Slavonic,Church Slavonic,Old Bulgarian","Oromo","Oriya","Ossetian, Ossetic","Panjabi, Punjabi","Pāli","Persian (Farsi)","Polish","Pashto, Pushto","Portuguese","Quechua","Romansh","Kirundi","Romanian","Russian","Sanskrit (Saṁskṛta)","Sardinian","Sindhi","Northern Sami","Samoan","Sango","Serbian","Scottish Gaelic, Gaelic","Shona","Sinhala, Sinhalese","Slovak","Slovene","Somali","Southern Sotho","Spanish, Castilian","Sundanese","Swahili","Swati","Swedish","Tamil","Telugu","Tajik","Thai","Tigrinya","Tibetan Standard, Tibetan, Central","Turkmen","Tagalog","Tswana","Tonga (Tonga Islands)","Turkish","Tsonga","Tatar","Twi","Tahitian","Uyghur, Uighur","Ukrainian","Urdu","Uzbek","Venda","Vietnamese","Volapük","Walloon","Welsh","Wolof","Western Frisian","Xhosa","Yiddish","Yoruba","Zhuang, Chuang","Zulu"];

  @ViewChild('repoTreeModal', { static: true })
  modal: PublishModalComponent;

  constructor(private repoService: RepoService) { }

  ngOnInit() {
  }

  public getTabVcs(form: FormGroup) {
    return this.publishForm.controls['vcs'].value;
  }

  public getRepoTree() {
    if(!this.publishForm.controls['url'].valid || !(this.publishForm.controls['pubSnap'].value == 'true')) {
      return;
    }

    this.loadingRepoTreeError = false;
    this.loadingRepoTree = true;
    this.repoTree = undefined;
    this.selectedCommit = undefined;
    this.publishForm.controls['snapshot'].setValue('');

    this.repoService.getRepoTree({
      repoUrl: this.publishForm.controls['url'].value,
      versionControl: this.publishForm.controls['vcs'].value
    }).then(result => {
      this.repoTree = result;
      this.loadingRepoTree = false;
      this.selectedCommit = this.repoTree.repoWeeks[0].commits[0];
      this.selectedCommit.selected = true;
      this.publishForm.controls['snapshot'].setValue(this.selectedCommit.commitId);
    }).catch(result => {
      this.loadingRepoTree = false;
      this.loadingRepoTreeError = true;
    });
  }

  public selectCommit(commit: RepoCommit) {
    this.selectedCommit.selected = false;
    commit.selected = true;
    this.selectedCommit = commit;
    this.publishForm.controls['snapshot'].setValue(commit.commitId);
  }

  public getToolTip(): string {
    if (!this.loadingRepoTree && !this.loadingRepoTreeError) {
      return "Enter repository URL to enable snapshot selection"
    } else if (!this.loadingRepoTree && this.loadingRepoTreeError) {
      return "URL does not point to a public repository"
    }
  }

  public publishSnapshot(): boolean {
    if(this.publishForm.controls['pubSnap'].value == 'true') {
      return true;
    }
    return false;
  }

  public getFullRepoText() {
    if(this.publishForm.controls['vcs'].value == 'git') {
      return "Only the contents of the .git folder are published"
    } else if (this.publishForm.controls['vcs'].value == 'svn') {
      return "The full contents of the remote repository are published"
    }
  }

  public getSnapshotText() {
    if(this.publishForm.controls['vcs'].value == 'git') {
      return "Only the files of the selected snapshot are published, excluding the .git folder"
    } else if (this.publishForm.controls['vcs'].value == 'svn') {
      return "Only the files in the working copy of the selected snapshot are published, excluding the .svn folder"
    }
  }

}

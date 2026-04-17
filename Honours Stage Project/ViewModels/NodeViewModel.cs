using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.Services;

namespace Honours_Stage_Project.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private readonly INodeConnectionService _connectionService;
        private bool _isDefaultOutgoingVisible = true;
        private bool _isRootNode = false;
        private ILuaStubValidationService _luaStubValidationService;
        private bool _hasLuaValidationError;
        private string _luaValidationErrorText = string.Empty;

        public ObservableCollection<ConnectionViewModel> ConnectionComponents { get; }
            = new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<AttributeModel> Attributes => Model.Attributes;
        public NodeModel Model { get; }

        public string TextContent
        {
            get => Model.TextContent;
            set
            {
                if (Model.TextContent == value) return;
                Model.TextContent = value;
                OnPropertyChanged(nameof(TextContent));
                ValidateLuaText();
            }
        }

        public bool HasLuaValidationError
        {
            get => _hasLuaValidationError;
            private set
            {
                if (_hasLuaValidationError == value) return;
                _hasLuaValidationError = value;
                OnPropertyChanged(nameof(HasLuaValidationError));
            }
        }

        public bool IsRootNode
        {
            get => _isRootNode;
            set
            {
                if (_isRootNode == value) return;
                _isRootNode = value;
                OnPropertyChanged(nameof(IsRootNode));
            }
        }

        public string LuaValidationErrorText
        {
            get => _luaValidationErrorText;
            private set
            {
                if (_luaValidationErrorText == value) return;
                _luaValidationErrorText = value;
                OnPropertyChanged(nameof(LuaValidationErrorText));
            }
        }

        public double X
        {
            get => Model.X;
            set
            {
                if (Model.X == value) return;
                Model.X = value;
                OnPropertyChanged(nameof(X));
            }
        }

        public double Y
        {
            get => Model.Y;
            set
            {
                if (Model.Y == value) return;
                Model.Y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public bool IsDefaultOutgoingVisible
        {
            get => _isDefaultOutgoingVisible;
            private set
            {
                if (_isDefaultOutgoingVisible == value) return;
                _isDefaultOutgoingVisible = value;
                OnPropertyChanged(nameof(IsDefaultOutgoingVisible));
            }
        }

        public Size Size
        {
            get => Model.Size;
            set
            {
                if (Model.Size == value) return;
                Model.Size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public ICommand AddConnectionComponentCommand { get; }
        public ICommand AddIncomingConnectionCommand { get; }
        public ICommand AddDefaultConnectionCommand { get; }
        public ICommand AddAttributeCommand { get; }
        public ICommand RemoveAttributeCommand { get; }
        public ICommand RemoveComponentCommand { get; }

        public NodeViewModel(
            NodeModel model,
            INodeConnectionService connectionService,
            ILuaStubValidationService luaStubValidationService = null)
        {
            Model = model;
            _connectionService = connectionService;

            AddConnectionComponentCommand = new RelayCommand(_ => AddConnectionComponent());
            AddIncomingConnectionCommand = new RelayCommand(_ => AddIncomingConnection());
            AddDefaultConnectionCommand = new RelayCommand(_ => AddDefaultConnection());
            AddAttributeCommand = new RelayCommand(_ => AddAttribute());
            RemoveAttributeCommand = new RelayCommand(attr => RemoveAttribute(attr as AttributeModel));
            RemoveComponentCommand = new RelayCommand(component => RemoveComponent(component as ConnectionViewModel));

            if (Model.ID == 0)
                IsRootNode = true;

            foreach (var component in Model.ConnectionComponents)
            {
                if (component.ID == 0)
                    continue;

                ConnectionComponents.Add(new ConnectionViewModel(component, Model.ID, _connectionService));
            }

            if (ConnectionComponents.Count > 0)
                IsDefaultOutgoingVisible = false;

            ConfigureLuaValidation(luaStubValidationService);
        }

        public void ConfigureLuaValidation(ILuaStubValidationService luaStubValidationService)
        {
            _luaStubValidationService = luaStubValidationService;

            ValidateLuaText();
        }

        private void ValidateLuaText()
        {
            if (_luaStubValidationService == null || string.IsNullOrWhiteSpace(TextContent))
            {
                SetLuaValidationErrors(new List<string>());
                return;
            }

            try
            {
                List<LuaValidationResult> validations = _luaStubValidationService.ValidateLua(TextContent);
                var errors = new List<string>();

                foreach (var validation in validations)
                {
                    if (validation == null || validation.IsValid)
                        continue;

                    foreach (var error in validation.Errors)
                        errors.Add(error);
                }

                SetLuaValidationErrors(errors);
            }
            catch (Exception e)
            {
                SetLuaValidationErrors(new List<string> { "Validation failed: " + e.Message });
            }
        }

        private void SetLuaValidationErrors(List<string> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                HasLuaValidationError = false;
                LuaValidationErrorText = string.Empty;
                return;
            }

            HasLuaValidationError = true;
            LuaValidationErrorText = string.Join(Environment.NewLine, errors.Distinct());
        }

        private void AddIncomingConnection()
        {
            if (IsRootNode)
                return;

            if(_connectionService.Connections.Where(c => c.TargetNodeId == Model.ID).Any() && _connectionService.PendingOutgoing == (-1, -1, -1))
            {
                _connectionService.RemoveIncoming(Model.ID);
                return;
            }

            _connectionService.AddIncoming(Model.ID);
        }

        private void AddConnectionComponent()
        {
            if (IsRootNode)
                return;

            RemoveDefaultConnection();

            var componentModel = Model.AddConnectionComponent();
            ConnectionComponents.Add(new ConnectionViewModel(componentModel, Model.ID, _connectionService));
        }

        private void AddDefaultConnection()
        {
            if (Model.GetComponentConnection(0) != null)
            {
                _connectionService.RemoveOutgoing(Model.ID, 0, 0);
                Model.RemoveConnectionComponent(0);
                return;
            }

            _connectionService.AddOutgoing(Model.ID, 0, 0);
            Model.AddDefaultConnectionComponent();
        }

        private void AddAttribute()
        {
            if (IsRootNode)
                return;

            Attributes.Add(new AttributeModel { Id = Attributes.Count });
        }

        private void RemoveAttribute(AttributeModel attribute)
        {
            if (IsRootNode)
                return;
            Attributes.Remove(attribute);
        }

        private void RemoveDefaultConnection()
        {
            _connectionService.RemoveOutgoing(Model.ID, 0, 0);
            ConnectionComponents.Remove(ConnectionComponents.FirstOrDefault(c => c.ID == 0));
            Model.RemoveConnectionComponent(0);
            IsDefaultOutgoingVisible = false;
        }

        private void RemoveComponent(ConnectionViewModel component)
        {
            if (IsRootNode || component == null)
                return;

            _connectionService.RemoveConnectionComponent(Model.ID, component.ID);

            ConnectionComponents.Remove(component);
            Model.RemoveConnectionComponent(component.ID);

            RenumberConnectionComponents();

            IsDefaultOutgoingVisible = ConnectionComponents.Count == 0;
        }

        private void RenumberConnectionComponents()
        {
            int nextId = 1;

            foreach (ConnectionModel connectionModel in Model.ConnectionComponents.OrderBy(c => c.ID))
            {
                if (connectionModel.ID == 0)
                    continue;

                connectionModel.ID = nextId;
                nextId++;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
